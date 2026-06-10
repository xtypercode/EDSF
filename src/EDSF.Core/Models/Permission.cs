using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class Permission : ISoftDeletable
{
    public int Id { get; set; }
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;
    [Required, StringLength(100)]
    public string Module { get; set; } = string.Empty;
    public bool CanRead { get; set; } = true;
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
