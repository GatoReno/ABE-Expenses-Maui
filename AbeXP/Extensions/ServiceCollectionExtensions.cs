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
        services.AddSingleton<HomeViewModel>();
        services.AddTransient<MainPageViewModel>();
        services.AddTransient<ExpenseFormViewModel>();
        services.AddSingleton<FinantialChartsViewModel>();


        // Views
        services.AddTransient<LoginView>();
        services.AddTransient<HomeView>();
        services.AddSingleton<MainPage>();
        services.AddTransient<ExpenseFormView>();
        services.AddSingleton<FinantialChartsPage>();


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
        services.AddSingleton<IExpenseRepository, ExpenseRepository>(sp =>
        {
            var fibInstanceService = sp.GetRequiredService<IFibInstance>();
            return new ExpenseRepository(fibInstanceService, FirebaseConstants.EXPENSES_COLLECTION);
        });

        return services;
    }
}