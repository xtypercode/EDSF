using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;
using EDSF.Core.Enums;

namespace EDSF.Core.Models;

public class InvoiceLine : ISoftDeletable
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;
    [StringLength(50)]
    public string? ProductCode { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public IvaRate TaxRate { get; set; } = IvaRate.Standard;
    public decimal TaxBase { get; set; }
    public decimal TaxAmount { get; set; }
    public ExemptionReason? ExemptionReason { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
