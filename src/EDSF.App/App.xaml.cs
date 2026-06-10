namespace EDSF.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage()) { Title = "EDSF" };
#if WINDOWS
        window.Created += (s, e) =>
        {
            var handler = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (handler is not null)
            {
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(handler);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                appWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);
            }
        };
#endif
        return window;
    }
}
