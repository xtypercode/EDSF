using System.Reflection;
using System.Text;
using EDSF.Core.Interfaces;

namespace EDSF.Core.Services;

public class CsvExportServiceWrapper : ICsvExportService
{
    public string Export<T>(IEnumerable<T> data, string[] columns)
    {
        var items = data.ToList();
        if (items.Count == 0) return "No data";

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (columns.Length > 0)
            props = props.Where(p => columns.Contains(p.Name)).ToArray();

        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", props.Select(p => $"\"{p.Name}\"")));

        foreach (var item in items)
        {
            var values = props.Select(p =>
            {
                var val = p.GetValue(item)?.ToString() ?? "";
                if (val.Contains(",") || val.Contains("\"") || val.Contains("\n"))
                    val = $"\"{val.Replace("\"", "\"\"")}\"";
                return val;
            });
            sb.AppendLine(string.Join(",", values));
        }

        return sb.ToString();
    }
}
