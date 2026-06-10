namespace EDSF.App;

public class ToastService
{
    public static ToastService Instance { get; set; } = new();
    public event Action<string, string>? OnShow;
    public void Show(string message, string type = "success") => OnShow?.Invoke(message, type);
}
