using System.Text.Json;
using BIRPOSSystem.Data;
using BIRPOSSystem.Models;
using BIRPOSSystem.Models.Compliance;
using BIRPOSSystem.Models.Sales;
using BIRPOSSystem.Models.Sync;
using BIRPOSSystem.Models.Tenancy;
using BIRPOSSystem.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BIRPOSSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/cash-shifts")]
public sealed class CashShiftController(ApplicationDbContext db) : ControllerBase
{
    private static readonly CashMovementType[] ManualMovementTypes =
    [
        CashMovementType.CashIn,
        CashMovementType.CashOut,
        CashMovementType.Drop,
        CashMovementType.Payout
    ];

    [HttpGet("workspace")]
    public async Task<CashShiftWorkspaceDto> GetWorkspaceAsync()
    {
        var register = await GetRegisterAsync();
        var shift = await db.CashShifts
            .Where(x => x.TerminalId == register.Terminal.Id && x.Status == ShiftStatus.Open)
            .OrderByDescending(x => x.OpenedAt)
            .FirstOrDefaultAsync();

        shift ??= await db.CashShifts
            .Where(x => x.TerminalId == register.Terminal.Id)
            .OrderByDescending(x => x.OpenedAt)
            .FirstOrDefaultAsync();

        var summary = await BuildSummaryAsync(shift, register.Branch, register.Terminal);
        var movements = await GetMovementDtosAsync(shift?.Id, register.Terminal.Id, register.Terminal.Code);

        return new CashShiftWorkspaceDto(summary, movements);
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenAsync([FromBody] OpenCashShiftRequest request)
    {
        if (request.OpeningCash < 0)
        {
            return BadRequest("Opening cash cannot be negative.");
        }

        var cashierName = string.IsNullOrWhiteSpace(request.CashierName)
            ? User.Identity?.Name ?? "Cashier"
            : request.CashierName.Trim();

        var register = await GetRegisterAsync();
        var hasOpenShift = await db.CashShifts.AnyAsync(x =>
            x.TerminalId == register.Terminal.Id &&
            x.Status == ShiftStatus.Open);

        if (hasOpenShift)
        {
            return Conflict("This terminal already has an open shift.");
        }

        var shift = new CashShift
        {
            BranchId = register.Branch.Id,
            TerminalId = register.Terminal.Id,
            CashierName = cashierName,
            OpeningCash = request.OpeningCash,
            Status = ShiftStatus.Open
        };

        var movement = CreateMovement(
            shift,
            register.Branch,
            register.Terminal,
            CashMovementType.OpeningFloat,
            request.OpeningCash,
            "Drawer opened",
            cashierName);

        db.CashShifts.Add(shift);
        db.CashMovements.Add(movement);
        AddAudit(cashierName, "CashShiftOpened", nameof(CashShift), shift.Id.ToString(), $"{register.Terminal.Code} opened with {request.OpeningCash:N2}");
        AddOutbox(nameof(CashShift), shift.Id.ToString(), new
        {
            shift.Id,
            register.Terminal.Code,
            shift.OpenedAt,
            shift.OpeningCash,
            shift.CashierName,
            Status = shift.Status.ToString()
        });

        await db.SaveChangesAsync();

        var summary = await BuildSummaryAsync(shift, register.Branch, register.Terminal);
        return Ok(new CashShiftActionResponse("Cash shift opened.", summary));
    }

    [HttpPost("{shiftId:guid}/movements")]
    public async Task<IActionResult> AddMovementAsync(Guid shiftId, [FromBody] AddCashMovementRequest request)
    {
        if (request.Amount <= 0)
        {
            return BadRequest("Movement amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return BadRequest("Reason is required.");
        }

        if (!TryParseMovementType(request.Type, out var type) || !ManualMovementTypes.Contains(type))
        {
            return BadRequest("Movement type must be CashIn, CashOut, Drop, or Payout.");
        }

        var shift = await db.CashShifts
            .Include(x => x.Branch)
            .Include(x => x.Terminal)
            .FirstOrDefaultAsync(x => x.Id == shiftId);

        if (shift is null)
        {
            return NotFound("Cash shift was not found.");
        }

        if (shift.Status != ShiftStatus.Open)
        {
            return BadRequest("Cannot add movements to a closed shift.");
        }

        var userName = User.Identity?.Name ?? shift.CashierName;
        var movement = CreateMovement(
            shift,
            shift.Branch!,
            shift.Terminal!,
            type,
            request.Amount,
            request.Reason.Trim(),
            userName);

        db.CashMovements.Add(movement);
        AddAudit(userName, "CashMovementAdded", nameof(CashMovement), movement.Id.ToString(), $"{MovementLabel(type)} {request.Amount:N2}: {movement.Reason}");
        AddOutbox(nameof(CashMovement), movement.Id.ToString(), new
        {
            movement.Id,
            movement.CashShiftId,
            movement.PostedAt,
            Type = movement.Type.ToString(),
            movement.Amount,
            movement.Reason,
            movement.UserName
        });

        await db.SaveChangesAsync();

        var summary = await BuildSummaryAsync(shift, shift.Branch!, shift.Terminal!);
        return Ok(new CashShiftActionResponse($"{MovementLabel(type)} recorded.", summary));
    }

    [HttpPost("{shiftId:guid}/close")]
    public async Task<IActionResult> CloseAsync(Guid shiftId, [FromBody] CloseCashShiftRequest request)
    {
        if (request.ClosingCash < 0)
        {
            return BadRequest("Closing cash cannot be negative.");
        }

        var shift = await db.CashShifts
            .Include(x => x.Branch)
            .Include(x => x.Terminal)
            .FirstOrDefaultAsync(x => x.Id == shiftId);

        if (shift is null)
        {
            return NotFound("Cash shift was not found.");
        }

        if (shift.Status != ShiftStatus.Open)
        {
            return BadRequest("Cash shift is already closed.");
        }

        var userName = User.Identity?.Name ?? shift.CashierName;
        shift.ClosingCash = request.ClosingCash;
        shift.ClosedAt = DateTime.UtcNow;
        shift.Status = ShiftStatus.Closed;
        shift.UpdatedAt = DateTime.UtcNow;

        var reason = string.IsNullOrWhiteSpace(request.Notes)
            ? "Drawer closed"
            : request.Notes.Trim();

        var movement = CreateMovement(
            shift,
            shift.Branch!,
            shift.Terminal!,
            CashMovementType.ClosingCount,
            request.ClosingCash,
            reason,
            userName);

        db.CashMovements.Add(movement);
        AddAudit(userName, "CashShiftClosed", nameof(CashShift), shift.Id.ToString(), $"{shift.Terminal!.Code} closed with {request.ClosingCash:N2}");
        AddOutbox(nameof(CashShift), shift.Id.ToString(), new
        {
            shift.Id,
            shift.Terminal.Code,
            shift.OpenedAt,
            shift.ClosedAt,
            shift.OpeningCash,
            shift.ClosingCash,
            Status = shift.Status.ToString()
        });

        await db.SaveChangesAsync();

        var summary = await BuildSummaryAsync(shift, shift.Branch!, shift.Terminal!);
        return Ok(new CashShiftActionResponse("Cash shift closed.", summary));
    }

    private async Task<RegisterContext> GetRegisterAsync()
    {
        var branch = await db.Branches.OrderBy(x => x.CreatedAt).FirstAsync();
        var terminal = await db.PosTerminals
            .Where(x => x.IsActive)
            .OrderBy(x => x.CreatedAt)
            .FirstAsync();

        return new RegisterContext(branch, terminal);
    }

    private async Task<CashShiftSummaryDto> BuildSummaryAsync(CashShift? shift, Branch branch, PosTerminal terminal)
    {
        if (shift is null)
        {
            return new CashShiftSummaryDto(
                null,
                false,
                branch.Name,
                terminal.Code,
                "No open shift",
                null,
                null,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                null,
                null,
                "No Open Shift");
        }

        var sales = await db.SalesTransactions
            .AsNoTracking()
            .Include(x => x.Payments)
            .Where(x => x.ShiftId == shift.Id && x.Status == SaleStatus.Completed)
            .ToListAsync();

        var cashSales = sales.Sum(CashImpact);

        var movements = await db.CashMovements
            .AsNoTracking()
            .Where(x => x.CashShiftId == shift.Id)
            .ToListAsync();

        var cashIn = movements.Where(x => x.Type == CashMovementType.CashIn).Sum(x => x.Amount);
        var cashOut = movements.Where(x => x.Type == CashMovementType.CashOut).Sum(x => x.Amount);
        var drops = movements.Where(x => x.Type == CashMovementType.Drop).Sum(x => x.Amount);
        var payouts = movements.Where(x => x.Type == CashMovementType.Payout).Sum(x => x.Amount);
        var expectedCash = shift.OpeningCash + cashSales + cashIn - cashOut - drops - payouts;
        var closingCash = shift.Status == ShiftStatus.Closed ? shift.ClosingCash : (decimal?)null;

        return new CashShiftSummaryDto(
            shift.Id,
            shift.Status == ShiftStatus.Open,
            branch.Name,
            terminal.Code,
            shift.CashierName,
            shift.OpenedAt,
            shift.ClosedAt,
            shift.OpeningCash,
            cashSales,
            cashIn,
            cashOut,
            drops,
            payouts,
            expectedCash,
            closingCash,
            closingCash is null ? null : closingCash.Value - expectedCash,
            shift.Status.ToString());
    }

    private async Task<IReadOnlyList<CashMovementDto>> GetMovementDtosAsync(Guid? shiftId, Guid terminalId, string terminalCode)
    {
        var query = db.CashMovements.AsNoTracking().Where(x => x.TerminalId == terminalId);
        if (shiftId is not null)
        {
            query = query.Where(x => x.CashShiftId == shiftId);
        }

        var movements = await query
            .OrderByDescending(x => x.PostedAt)
            .Take(80)
            .ToListAsync();

        return movements
            .Select(x => ToDto(x, terminalCode))
            .ToList();
    }

    private static CashMovement CreateMovement(
        CashShift shift,
        Branch branch,
        PosTerminal terminal,
        CashMovementType type,
        decimal amount,
        string reason,
        string userName) =>
        new()
        {
            CashShiftId = shift.Id,
            CashShift = shift,
            BranchId = branch.Id,
            TerminalId = terminal.Id,
            PostedAt = DateTime.UtcNow,
            Type = type,
            Amount = amount,
            Reason = reason,
            UserName = userName,
            ReferenceNumber = $"{terminal.Code}-{DateTime.UtcNow:yyyyMMddHHmmss}"
        };

    private void AddAudit(string userName, string action, string entityName, string entityId, string details)
    {
        db.AuditLogs.Add(new AuditLog
        {
            UserName = userName,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Details = details
        });
    }

    private void AddOutbox(string itemType, string itemId, object payload)
    {
        db.SyncOutboxItems.Add(new SyncOutboxItem
        {
            ItemType = itemType,
            ItemId = itemId,
            PayloadJson = JsonSerializer.Serialize(payload)
        });
    }

    private static CashMovementDto ToDto(CashMovement movement, string terminalCode) =>
        new(
            movement.Id,
            movement.CashShiftId,
            movement.PostedAt,
            terminalCode,
            movement.Type.ToString(),
            MovementLabel(movement.Type),
            movement.UserName,
            movement.Amount,
            SignedAmount(movement.Type, movement.Amount),
            movement.Reason,
            movement.ReferenceNumber);

    private static decimal CashImpact(SalesTransaction sale)
    {
        var cashTendered = sale.Payments
            .Where(x => x.Method.Equals("Cash", StringComparison.OrdinalIgnoreCase))
            .Sum(x => x.Amount);

        return Math.Max(0, cashTendered - sale.ChangeDue);
    }

    private static decimal SignedAmount(CashMovementType type, decimal amount) =>
        type is CashMovementType.CashOut or CashMovementType.Drop or CashMovementType.Payout
            ? -amount
            : amount;

    private static bool TryParseMovementType(string value, out CashMovementType type)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            type = default;
            return false;
        }

        var normalized = value.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
        return Enum.TryParse(normalized, ignoreCase: true, out type);
    }

    private static string MovementLabel(CashMovementType type) =>
        type switch
        {
            CashMovementType.OpeningFloat => "Opening Float",
            CashMovementType.CashIn => "Cash In",
            CashMovementType.CashOut => "Cash Out",
            CashMovementType.Drop => "Cash Drop",
            CashMovementType.Payout => "Payout",
            CashMovementType.ClosingCount => "Closing Count",
            _ => type.ToString()
        };

    private sealed record RegisterContext(Branch Branch, PosTerminal Terminal);
}
