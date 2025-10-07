using AbeXP.Abstractions.Services;
using AbeXP.Interfaces;
using AbeXP.UseCases.Plugins;
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


        // Temporary page while we check login
        MainPage = new ContentPage
        {
            Content = new ActivityIndicator
            {
                IsRunning = true,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }
        };

        CheckLoginAsync();
    }

    public void SetNewShellPage()
    {
        MainPage = _serviceProvider.GetService<AppShell>();
    }

    public void SetLoginPage()
    {
        MainPage = _serviceProvider.GetService<LoginView>();
    }

    private async void CheckLoginAsync()
    {
        var userSession = _serviceProvider.GetService<IUserSession>();
        if (await userSession.IsLoggedInAsync())
        {
            if(!await userSession.IsSessionValid())
            {
                userSession.SignOut();
                SetLoginPage();
            }

            SetNewShellPage();
        }
        else
        {
            SetLoginPage();
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

    protected async override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);
    }
}

