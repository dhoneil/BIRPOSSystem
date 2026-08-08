using BIRPOSSystem.Data;
using BIRPOSSystem.Models.Catalog;
using BIRPOSSystem.Models.Compliance;
using BIRPOSSystem.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BIRPOSSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/products")]
public sealed class ProductController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync() =>
        await db.Products
            .Include(x => x.Category)
            .OrderBy(x => x.Category!.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new ProductDto(
                x.Id,
                x.Sku,
                x.Barcode,
                x.Name,
                x.Category!.Name,
                x.Cost,
                x.Price,
                x.QuantityOnHand,
                x.ReorderPoint,
                x.IsVatExempt,
                x.IsActive))
            .ToListAsync();

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync([FromBody] UpsertProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("SKU and name are required.");
        }

        if (request.CategoryId == Guid.Empty)
        {
            return BadRequest("Category is required.");
        }

        if (request.Price < 0)
        {
            return BadRequest("Price cannot be negative.");
        }

        var sku = request.Sku.Trim();
        if (await db.Products.AnyAsync(x => x.Sku == sku))
        {
            return Conflict("SKU already exists.");
        }

        var product = new Product
        {
            Sku = sku,
            Barcode = string.IsNullOrWhiteSpace(request.Barcode) ? sku : request.Barcode.Trim(),
            Name = request.Name.Trim(),
            CategoryId = request.CategoryId,
            Price = request.Price,
            Cost = request.Cost,
            QuantityOnHand = request.QuantityOnHand,
            ReorderPoint = request.ReorderPoint,
            IsVatExempt = request.IsVatExempt,
            IsActive = request.IsActive,
            TrackInventory = true
        };

        db.Products.Add(product);
        db.AuditLogs.Add(new AuditLog
        {
            UserName = User.Identity?.Name ?? "System",
            Action = "ProductCreated",
            EntityName = nameof(Product),
            EntityId = product.Id.ToString(),
            Details = $"{product.Sku} - {product.Name}"
        });

        await db.SaveChangesAsync();
        return Created($"/api/products/{product.Id}", product.Id);
    }
}
