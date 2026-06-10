using EDSF.Core.Enums;
using EDSF.Core.Interfaces;
using EDSF.Core.Models;

namespace EDSF.Core.Services;

public class SeriesManager
{
    private readonly IUnitOfWork _uow;

    public SeriesManager(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<string> NextNumber(DocumentType docType, int? fiscalYear = null)
    {
        var year = fiscalYear ?? DateTime.UtcNow.Year;
        var prefix = docType.ToString();
        var seriesCode = $"{prefix}-{year}";

        var series = (await _uow.InvoiceSeries.FindAsync(s =>
            s.Series == seriesCode && s.FiscalYear == year && s.IsActive)).FirstOrDefault();

        if (series == null)
        {
            series = new InvoiceSeries
            {
                Series = seriesCode,
                DocumentType = docType,
                FiscalYear = year,
                CurrentNumber = 1
            };
            await _uow.InvoiceSeries.AddAsync(series);
        }

        var number = series.CurrentNumber;
        series.CurrentNumber++;
        await _uow.SaveChangesAsync();

        return $"{prefix}-{year}-{number:D4}";
    }
}
