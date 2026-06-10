using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class CreditNote : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Number { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
    [StringLength(500)]
    public string? Reason { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
