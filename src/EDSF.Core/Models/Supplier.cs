using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class Supplier : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    [StringLength(20)]
    public string? Nif { get; set; }
    [StringLength(50)]
    public string? Phone { get; set; }
    [StringLength(200)]
    public string? Email { get; set; }
    [StringLength(500)]
    public string? Address { get; set; }
    [StringLength(100)]
    public string? ContactPerson { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
