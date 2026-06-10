namespace EDSF.Core.Interfaces;

public interface ICsvExportService
{
    string Export<T>(IEnumerable<T> data, string[] columns);
}
