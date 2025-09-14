using System;
using AbeXP.Abstractions.Interfaces;
using AbeXP.Abstractions.Services;
using AbeXP.ViewModels;
using AbeXP.Views;

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

        // Views
        services.AddTransient<LoginView>();
        services.AddTransient<HomeView>();

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

        // Ejemplo: un repositorio de usuarios
        //services.AddTransient<IUserRepository, UserRepository>();

        return services;
    }
}