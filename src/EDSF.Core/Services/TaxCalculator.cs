using EDSF.Core.Enums;

namespace EDSF.Core.Services;

public static class TaxCalculator
{
    public static decimal GetRate(IvaRate rate) => rate switch
    {
        IvaRate.Standard => 0.14m,
        IvaRate.Reduced => 0.05m,
        IvaRate.Exempt => 0m,
        IvaRate.OutsideScope => 0m,
        _ => 0m
    };

    public static decimal CalcTax(decimal netAmount, IvaRate rate) =>
        netAmount * GetRate(rate);

    public static decimal CalcTaxBase(decimal unitPrice, int quantity, decimal discount = 0) =>
        (unitPrice * quantity) - discount;

    public static decimal CalcStampTax(decimal totalAmount, DocumentType docType) => docType switch
    {
        DocumentType.FT or DocumentType.FR => totalAmount * 0.001m,
        DocumentType.ND => totalAmount * 0.01m,
        _ => 0m
    };
}
