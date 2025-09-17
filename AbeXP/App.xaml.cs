using AbeXP.Abstractions.Services;
using AbeXP.Interfaces;
using AbeXP.ViewModels;
using AbeXP.ViewModels.AbeXP.ViewModels;
using AbeXP.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace AbeXP;

public partial class App : Application
{
    public static IAlertService Alert;
    private static App instance;
    public static App Instance { get { return instance; } }

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Alert = serviceProvider.GetService<IAlertService>();
        ConfigureCulture(serviceProvider.GetService<ISettingsService>());


        MainPage = new AppShell();

        //bool isLogged = Preferences.Get("IsLogged", false);
        bool isLogged = Preferences.Get("IsLogged", true);
        if (isLogged)
        {
            MainPage = new AppShell();
        }
        else
        {
            LoginPageNavigation();
        }
    }

    private void ConfigureCulture(ISettingsService settingsService)
    {
        var culture = new CultureInfo(settingsService.Culture);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    public void LoginPageNavigation()
    {
        var authService = new FirebaseAuthService(); // tu implementación de IFibAuthLog
        var loginViewModel = new LoginViewModel(authService);
        MainPage = new MainPage(new MainPageViewModel());
        // MainPage = new LoginPage(new LoginViewModel(CrossFingerprint.Current, UserDialogs.Instance));
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

