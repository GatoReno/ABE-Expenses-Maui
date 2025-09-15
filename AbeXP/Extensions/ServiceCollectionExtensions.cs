using AbeXP.Abstractions.Interfaces;
using AbeXP.Abstractions.Services;
using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Services;
using AbeXP.ViewModels;
using AbeXP.ViewModels.AbeXP.ViewModels;
using AbeXP.Views;
using CommunityToolkit.Maui;
using Firebase.Auth;

namespace AbeXP.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todas las Views y ViewModels de la app.
    /// </summary>
    public static IServiceCollection ConfigureViewsAndViewModels(this IServiceCollection services)
    {
        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<FinantialChartsViewModel>();
        services.AddTransient<ExpenseFormViewModel>();


        // Views
        services.AddTransient<LoginView>();
        services.AddTransient<HomeView>();
        services.AddTransient<FinantialChartsPage>();
        services.AddTransient<ExpenseFormView>();


        return services;
    }

    /// <summary>
    /// Registra los servicios de la app (APIs, repositorios, etc.).
    /// </summary>
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        //services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IFibAuthLog, FirebaseAuthService>();
        services.AddSingleton<IAlertService, AlertService>();
        services.AddSingleton<IFibInstance, FibInstance>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IExpenseRepository, ExpenseRepository>();

        return services;
    }
}