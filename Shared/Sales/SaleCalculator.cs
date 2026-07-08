namespace BIRPOSSystem.Shared.Sales;

public sealed class SaleCalculator
{
    public const decimal DefaultVatRate = 0.12m;

    public SaleCalculationResult Calculate(
        IEnumerable<SaleCalculationLine> sourceLines,
        decimal manualDiscountAmount,
        decimal vatRate = DefaultVatRate)
    {
        var inputLines = sourceLines.ToList();
        if (inputLines.Count == 0)
        {
            throw new InvalidOperationException("A sale must contain at least one line.");
        }

        if (manualDiscountAmount < 0)
        {
            throw new InvalidOperationException("Manual discount cannot be negative.");
        }

        var grossBeforeManualDiscount = inputLines.Sum(line => line.Quantity * line.UnitPrice);
        if (manualDiscountAmount > grossBeforeManualDiscount)
        {
            throw new InvalidOperationException("Manual discount cannot exceed sale total.");
        }

        var calculatedLines = new List<CalculatedSaleLine>();

        foreach (var line in inputLines)
        {
            if (line.Quantity <= 0)
            {
                throw new InvalidOperationException($"Quantity for {line.ProductName} must be greater than zero.");
            }

            if (line.UnitPrice < 0)
            {
                throw new InvalidOperationException($"Price for {line.ProductName} cannot be negative.");
            }

            var grossAmount = RoundMoney(line.Quantity * line.UnitPrice);
            var proportionalManualDiscount = grossBeforeManualDiscount == 0
                ? 0
                : RoundMoney(manualDiscountAmount * (grossAmount / grossBeforeManualDiscount));
            var discountAmount = RoundMoney(line.DiscountAmount + proportionalManualDiscount);
            var netAmount = RoundMoney(grossAmount - discountAmount);
            var vatAmount = line.IsVatExempt ? 0 : RoundMoney(netAmount - (netAmount / (1 + vatRate)));

            calculatedLines.Add(new CalculatedSaleLine(
                line.ProductId,
                line.Sku,
                line.ProductName,
                line.Quantity,
                line.UnitPrice,
                grossAmount,
                discountAmount,
                vatAmount,
                netAmount,
                line.IsVatExempt));
        }

        return new SaleCalculationResult(
            calculatedLines,
            calculatedLines.Sum(line => line.GrossAmount),
            calculatedLines.Sum(line => line.DiscountAmount),
            calculatedLines.Where(line => !line.IsVatExempt).Sum(line => line.NetAmount - line.VatAmount),
            calculatedLines.Sum(line => line.VatAmount),
            calculatedLines.Where(line => line.IsVatExempt).Sum(line => line.NetAmount),
            calculatedLines.Sum(line => line.NetAmount));
    }

    private static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
