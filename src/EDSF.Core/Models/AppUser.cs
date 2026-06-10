using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class AppUser : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required, StringLength(200)]
    public string DisplayName { get; set; } = string.Empty;
    [Required, StringLength(200)]
    public string Email { get; set; } = string.Empty;
    [Required, StringLength(500)]
    public string PasswordHash { get; set; } = string.Empty;
    [StringLength(50)]
    public string? Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
