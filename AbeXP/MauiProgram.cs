using AbeXP.Extensions;
using CommunityToolkit.Maui;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;

namespace AbeXP;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseMicrocharts()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIconsRegular");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
        builder.Services.ConfigureViewsAndViewModels();
		builder.Services.ConfigureServices();
        //        builder.Services.ConfigureServices();

        return builder.Build();
	}
}

