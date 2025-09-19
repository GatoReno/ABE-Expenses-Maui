using AbeXP.Abstractions.Services;
using AbeXP.Interfaces;
using AbeXP.ViewModels;
using AbeXP.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;

namespace AbeXP;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    public static IAlertService Alert;
    private static App instance;
    public static App Instance { get { return instance; } }

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        ConfigureCulture();

        instance = this;
        Alert = _serviceProvider.GetService<IAlertService>();

        bool isLogged = Preferences.Get("IsLogged", true);
        if (isLogged)
        {
            MainPage = new AppShell(_serviceProvider);
        }
        else
        {
            MainPage = _serviceProvider.GetService<LoginView>();
        }
    }

    private void ConfigureCulture()
    {
        var settings = _serviceProvider.GetService<ISettingsService>();

        var culture = new CultureInfo(settings.Culture);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    protected override void OnResume()
    {
        base.OnResume();
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);
    }
}

