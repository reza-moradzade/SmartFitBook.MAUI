using Microsoft.EntityFrameworkCore;
using SmartFitBook.MAUI.Data;
using SmartFitBook.MAUI.Services;

namespace SmartFitBook.MAUI;

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
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        // دیتابیس SQLite
        builder.Services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlite($"Data Source={AppDbContext.DbPath}"));

        // سرویس‌ها
        builder.Services.AddScoped<DataService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<WorkoutService>();
        builder.Services.AddScoped<FileService>();
        builder.Services.AddSingleton<WorkoutStateService>();

        return builder.Build();
    }
}