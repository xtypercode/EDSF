using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;
using EDSF.Core.Enums;

namespace EDSF.Core.Models;

public class Customer : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    [StringLength(20)]
    public string? Nif { get; set; }
    [StringLength(500)]
    public string? Address { get; set; }
    public Province? Province { get; set; }
    [StringLength(100)]
    public string? Municipality { get; set; }
    [StringLength(100)]
    public string? Commune { get; set; }
    [StringLength(50)]
    public string? Phone { get; set; }
    [StringLength(200)]
    public string? Email { get; set; }
    public CustomerType CustomerType { get; set; } = CustomerType.Company;
    public int PaymentTerms { get; set; } = 30;
    public decimal? CreditLimit { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
