using EDSF.App.Services;
using Microsoft.Extensions.Logging;

namespace EDSF.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        var apiUrl =
            Environment.GetEnvironmentVariable("EDSF_API_URL")
            ?? "http://localhost:5285/api/";

        builder.Services.AddSingleton<TokenStore>();
        builder.Services.AddTransient<AuthHeaderHandler>();

        builder.Services.AddHttpClient("Api", client =>
        {
            client.BaseAddress = new Uri(apiUrl);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddTransient(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            return factory.CreateClient("Api");
        });

        builder.Services.AddScoped<AuthService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
