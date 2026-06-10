using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;
using EDSF.Core.Enums;

namespace EDSF.Core.Models;

public class Invoice : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Number { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DocumentType DocumentType { get; set; } = DocumentType.FT;
    [StringLength(20)]
    public string Series { get; set; } = "";
    public Currency Currency { get; set; } = Currency.AOA;
    public decimal ExchangeRate { get; set; } = 1.0m;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    [StringLength(500)]
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxBase { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal WithholdingTax { get; set; }
    public decimal StampTax { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
