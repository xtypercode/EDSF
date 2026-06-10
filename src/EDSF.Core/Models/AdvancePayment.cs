using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class AdvancePayment : ISoftDeletable
{
    public int Id { get; set; }
    [StringLength(200)]
    public string? EmployeeName { get; set; }
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedReturnDate { get; set; }
    [StringLength(500)]
    public string? Reason { get; set; }
    public bool IsSettled { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
