using System.Globalization;
using System.Text;
using System.Xml;
using EDSF.Core.Enums;
using EDSF.Core.Models;

namespace EDSF.Core.Services;

public class ASaftGenerator
{
    public string Generate(
        CompanyData company,
        List<Invoice> invoices,
        List<Customer> customers,
        int fiscalYear)
    {
        var sb = new StringBuilder();
        var settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = false };
        using var writer = XmlWriter.Create(sb, settings);

        writer.WriteStartDocument();
        writer.WriteStartElement("AuditFile");
        writer.WriteAttributeString("xmlns", "urn:OECD:StandardAuditFile-Tax:AO_PT:v1.01_01");

        writer.WriteElementString("Header", null);
        writer.WriteElementString("CompanyName", company.Name);
        writer.WriteElementString("CompanyTaxID", company.Nif);
        writer.WriteElementString("CompanyAddress", BuildAddress(company));
        writer.WriteElementString("FiscalYear", fiscalYear.ToString());
        writer.WriteElementString("TaxAccountingBasis", company.TaxRegime.ToString());
        writer.WriteElementString("CurrencyCode", "AOA");
        writer.WriteElementString("DateCreated", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"));

        writer.WriteStartElement("Customers");
        foreach (var c in customers.Where(c => !string.IsNullOrEmpty(c.Nif)))
        {
            writer.WriteStartElement("Customer");
            writer.WriteElementString("CustomerTaxID", c.Nif);
            writer.WriteElementString("CompanyName", c.Name);
            writer.WriteElementString("AddressDetail", BuildCustomerAddress(c));
            writer.WriteElementString("SelfBillingIndicator", "0");
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        writer.WriteStartElement("TaxExemptionReasons");
        foreach (var reason in Enum.GetValues<ExemptionReason>())
        {
            writer.WriteStartElement("TaxExemptionReason");
            writer.WriteElementString("ExemptionReasonCode", reason.ToString());
            writer.WriteElementString("Description", GetExemptionReasonDescription(reason));
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        writer.WriteStartElement("TaxTable");
        foreach (var rate in new[] { IvaRate.Standard, IvaRate.Reduced, IvaRate.Exempt, IvaRate.OutsideScope })
        {
            var taxRate = TaxCalculator.GetRate(rate);
            writer.WriteStartElement("Tax");
            writer.WriteElementString("TaxType", "IVA");
            writer.WriteElementString("TaxCode", rate switch
            {
                IvaRate.Standard => "NOR",
                IvaRate.Reduced => "RED",
                IvaRate.Exempt => "ISE",
                IvaRate.OutsideScope => "OUT",
                _ => "NOR"
            });
            writer.WriteElementString("TaxPercentage", (taxRate * 100).ToString("F2", CultureInfo.InvariantCulture));
            writer.WriteElementString("TaxDescription", rate.ToString());
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        writer.WriteStartElement("SalesInvoices");
        foreach (var invoice in invoices)
        {
            writer.WriteStartElement("Invoice");
            writer.WriteElementString("InvoiceNo", invoice.Number);
            writer.WriteElementString("DocumentType", invoice.DocumentType.ToString());
            writer.WriteElementString("Series", invoice.Series);
            writer.WriteElementString("InvoiceDate", invoice.Date.ToString("yyyy-MM-dd"));
            writer.WriteElementString("CustomerTaxID", invoice.Customer?.Nif ?? "");
            writer.WriteElementString("CurrencyCode", invoice.Currency.ToString());
            writer.WriteElementString("ExchangeRate", invoice.ExchangeRate.ToString("F6", CultureInfo.InvariantCulture));
            writer.WriteElementString("NetTotal", invoice.TaxBase.ToString("F2", CultureInfo.InvariantCulture));
            writer.WriteElementString("TaxTotal", invoice.TaxAmount.ToString("F2", CultureInfo.InvariantCulture));
            writer.WriteElementString("GrossTotal", invoice.TotalAmount.ToString("F2", CultureInfo.InvariantCulture));
            writer.WriteElementString("WithholdingTax", invoice.WithholdingTax.ToString("F2", CultureInfo.InvariantCulture));
            writer.WriteElementString("StampTax", invoice.StampTax.ToString("F2", CultureInfo.InvariantCulture));

            writer.WriteStartElement("Lines");
            foreach (var line in invoice.Lines)
            {
                writer.WriteStartElement("Line");
                writer.WriteElementString("ProductCode", line.ProductCode ?? "");
                writer.WriteElementString("ProductDescription", line.Description);
                writer.WriteElementString("Quantity", line.Quantity.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("UnitPrice", line.UnitPrice.ToString("F2", CultureInfo.InvariantCulture));
                writer.WriteElementString("TaxBase", line.TaxBase.ToString("F2", CultureInfo.InvariantCulture));
                writer.WriteElementString("TaxAmount", line.TaxAmount.ToString("F2", CultureInfo.InvariantCulture));
                writer.WriteElementString("TaxRate", (TaxCalculator.GetRate(line.TaxRate) * 100).ToString("F2"));
                writer.WriteElementString("Discount", line.Discount.ToString("F2", CultureInfo.InvariantCulture));
                if (line.ExemptionReason.HasValue)
                    writer.WriteElementString("ExemptionReason", line.ExemptionReason.Value.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        writer.WriteEndElement();
        writer.WriteEndDocument();

        return sb.ToString();
    }

    private static string BuildAddress(CompanyData c)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(c.Address)) parts.Add(c.Address);
        if (!string.IsNullOrEmpty(c.Municipality)) parts.Add(c.Municipality);
        if (c.Province.HasValue) parts.Add(c.Province.Value.ToString());
        return string.Join(", ", parts);
    }

    private static string BuildCustomerAddress(Customer c)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(c.Address)) parts.Add(c.Address);
        if (!string.IsNullOrEmpty(c.Municipality)) parts.Add(c.Municipality);
        if (c.Province.HasValue) parts.Add(c.Province.Value.ToString());
        return string.Join(", ", parts);
    }

    private static string GetExemptionReasonDescription(ExemptionReason reason) => reason switch
    {
        Enums.ExemptionReason.M01 => "Operação não sujeita (arts. 7º, 8º e 9º CIVA)",
        Enums.ExemptionReason.M02 => "Operação isenta (arts. 14º a 20º CIVA)",
        Enums.ExemptionReason.M03 => "IVA autoliquidação (arts. 2º, 6º e 9º CIVA)",
        Enums.ExemptionReason.M04 => "Exportação (art. 14º CIVA)",
        Enums.ExemptionReason.M05 => "Operações assimiladas a exportação (art. 14º CIVA)",
        Enums.ExemptionReason.M06 => "Operações no âmbito do RITI",
        Enums.ExemptionReason.M07 => "Operações isentas não conferindo direito à dedução",
        Enums.ExemptionReason.M08 => "Transmissão de bens para outros Estados membros",
        Enums.ExemptionReason.M09 => "Aquisição intracomunitária de bens",
        Enums.ExemptionReason.M10 => "IVA diferido nas importações",
        Enums.ExemptionReason.M11 => "Operações de seguro, resseguro e capitais (art. 16º CIVA)",
        Enums.ExemptionReason.M12 => "Operações com IVA não dedutível",
        _ => reason.ToString()
    };
}
