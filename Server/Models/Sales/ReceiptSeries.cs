using BIRPOSSystem.Models.Common;
using BIRPOSSystem.Models.Tenancy;

namespace BIRPOSSystem.Models.Sales;

public sealed class ReceiptSeries : EntityBase
{
    public Guid TerminalId { get; set; }
    public PosTerminal? Terminal { get; set; }
    public string Prefix { get; set; } = "SI";
    public long CurrentNumber { get; set; }
    public long EndingNumber { get; set; } = 999999999;
    public bool IsActive { get; set; } = true;

    public string PeekNextInvoiceNumber() => $"{Prefix}-{CurrentNumber + 1:000000}";

    public string ConsumeNextInvoiceNumber()
    {
        CurrentNumber++;
        return $"{Prefix}-{CurrentNumber:000000}";
    }
}
