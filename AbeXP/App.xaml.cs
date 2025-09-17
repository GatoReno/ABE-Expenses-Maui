using AbeXP.Abstractions.Services;
using AbeXP.Interfaces;
using AbeXP.ViewModels;
using AbeXP.ViewModels.AbeXP.ViewModels;
using AbeXP.Views;
using Microsoft.Extensions.DependencyInjection;

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

        MainPage = new AppShell();

        bool isLogged = Preferences.Get("IsLogged", false);
        if (isLogged)
        {
            MainPage = new NavigationPage(new MainPage(new MainPageViewModel()));
        }
        else
        {
            LoginPageNavigation();
        }
    }

    public void LoginPageNavigation()
    {
        MainPage = new AppShell();
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

