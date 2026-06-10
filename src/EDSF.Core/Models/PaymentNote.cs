using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;
using EDSF.Core.Enums;

namespace EDSF.Core.Models;

public class PaymentNote : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Number { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? SupplierId { get; set; }
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    [StringLength(100)]
    public string? BankReference { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    [StringLength(500)]
    public string? Notes { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
