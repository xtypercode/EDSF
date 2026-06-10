using System.ComponentModel.DataAnnotations;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Models;

public class TransportGuide : ISoftDeletable
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Number { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    [StringLength(200)]
    public string? Origin { get; set; }
    [StringLength(200)]
    public string? Destination { get; set; }
    [StringLength(200)]
    public string? Carrier { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    [StringLength(500)]
    public string? Notes { get; set; }
    public ICollection<TransportGuideItem> Items { get; set; } = new List<TransportGuideItem>();

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}

public class TransportGuideItem
{
    public int Id { get; set; }
    public int TransportGuideId { get; set; }
    public TransportGuide TransportGuide { get; set; } = null!;
    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    [StringLength(20)]
    public string? Unit { get; set; }

    [ConcurrencyCheck] public int Version { get; set; }
}
