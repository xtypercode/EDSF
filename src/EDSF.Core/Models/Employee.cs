using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class Employee : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    [StringLength(100)]
    public string? Position { get; set; }
    [StringLength(100)]
    public string? Department { get; set; }
    [StringLength(50)]
    public string? Phone { get; set; }
    [StringLength(200)]
    public string? Email { get; set; }
    public DateTime? HireDate { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
