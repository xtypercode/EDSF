using System.ComponentModel.DataAnnotations;
using EDSF.Core.Enums;

namespace EDSF.Core.Models;

public class InvoiceSeries
{
    public int Id { get; set; }
    [Required, StringLength(20)]
    public string Series { get; set; } = "";
    public DocumentType DocumentType { get; set; } = DocumentType.FT;
    public int FiscalYear { get; set; }
    public int CurrentNumber { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ConcurrencyCheck] public int Version { get; set; }
}
