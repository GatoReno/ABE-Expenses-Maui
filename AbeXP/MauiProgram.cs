using AbeXP.Extensions;
using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AbeXP;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .UseLiveCharts()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIconsRegular");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
		builder.Services.ConfigureServices();
        builder.Services.ConfigureViewsAndViewModels();
        //        builder.Services.ConfigureServices();



#if ANDROID
        builder.Services.AddSingleton<AbeXP.Interfaces.IWidgetUpdater, AbeXP.Platforms.Android.Widget.Service.WidgetUpdater>();
#elif IOS
        builder.Services.AddSingleton<AbeXP.Interfaces.IWidgetUpdater, AbeXP.Platforms.iOS.Widget.Service.WidgetUpdater>();
#endif

        return builder.Build();
	}
}

