using AbeXP.Abstractions.Interfaces;
using AbeXP.Abstractions.Services;
using AbeXP.Interfaces;
using AbeXP.Services;
using AbeXP.ViewModels;
using AbeXP.ViewModels.AbeXP.ViewModels;
using AbeXP.Views;
using System;

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
        services.AddTransient<ExpenseFormViewModel>();
        

        // Views
        services.AddTransient<LoginView>();
        services.AddTransient<HomeView>();
        services.AddTransient<ExpenseFormView>();
        

        return services;
    }

    /// <summary>
    /// Registra los servicios de la app (APIs, repositorios, etc.).
    /// </summary>
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        // Ejemplo: un servicio de autenticación
        //services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IFibAuthLog, FirebaseAuthService>();
        services.AddSingleton<IAlertService, AlertService>();

        // Ejemplo: un repositorio de usuarios
        //services.AddTransient<IUserRepository, UserRepository>();

        return services;
    }
}