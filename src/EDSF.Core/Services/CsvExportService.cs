using System.Reflection;
using System.Text;

namespace EDSF.Core.Services;

public static class CsvExportService
{
    public static byte[] ExportToCsv<T>(IEnumerable<T> data, string[]? propertiesToInclude = null)
    {
        var items = data.ToList();
        if (items.Count == 0) return Encoding.UTF8.GetBytes("No data");

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (propertiesToInclude != null && propertiesToInclude.Length > 0)
            props = props.Where(p => propertiesToInclude.Contains(p.Name)).ToArray();

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

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
