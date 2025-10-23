using AbeXP.Abstractions.Interfaces;
using AbeXP.Abstractions.Services;
using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Services;
using AbeXP.Services.CatalogCache;
using AbeXP.UseCases;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;
using AbeXP.ViewModels;
using AbeXP.Views;
using CommunityToolkit.Maui;

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
        services.AddTransient<MainPageViewModel>();
        services.AddTransient<ExpenseFormViewModel>();
        services.AddTransient<IncomeFormViewModel>();
        services.AddTransient<FinantialChartsViewModel>();
        services.AddTransient<ProfileViewModel>();
        services.AddTransient<AppShellViewModel>();


        // Views
        services.AddTransient<LoginView>();
        services.AddTransient<MainPage>();
        services.AddTransient<ExpenseFormView>();
        services.AddTransient<IncomeFormView>();
        services.AddTransient<FinantialChartsPage>();
        services.AddTransient<ProfilePage>();
        services.AddTransient<AppShell>();


        return services;
    }

    /// <summary>
    /// Registra los servicios de la app (APIs, repositorios, etc.).
    /// </summary>
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<INavigationService, MauiNavigationService>();
        services.AddSingleton<IFibAuthLog, FirebaseAuthService>();
        services.AddSingleton<IAlertService, AlertService>();
        services.AddSingleton<IFibInstance, FibInstance>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IUserSession, UserSession>();
        services.AddSingleton<ICatalogCacheService, CatalogCacheService>();
        services.AddSingleton<ICatalogMetadataService, CatalogMetadataService>();

        services.AddSingleton<ITransactionsRepository, TransactionsRepository>(sp =>
        {
            var fibInstanceService = sp.GetRequiredService<IFibInstance>();
            return new TransactionsRepository(fibInstanceService, FirebaseConstants.TRANSACTIONS_COLLECTION);
        });

        services.AddSingleton<IIncomeRepository, IncomeRepository>(sp =>
        {
            var fibInstanceService = sp.GetRequiredService<IFibInstance>();
            return new IncomeRepository(fibInstanceService, FirebaseConstants.INCOMES_COLLECTION);
        });

        services.AddSingleton<TagsRepository>(sp =>
        {
            var fibInstanceService = sp.GetRequiredService<IFibInstance>();
            return new TagsRepository(fibInstanceService, FirebaseConstants.TAGS_COLLECTION);
        });

        services.AddSingleton<ITagsRepository>(sp =>
        {
            var inner = sp.GetRequiredService<TagsRepository>();
            var cacheService = sp.GetRequiredService<ICatalogCacheService>();
            var metadataService = sp.GetRequiredService<ICatalogMetadataService>();
            return new CachedTagsRepository(inner, cacheService, metadataService);
        });

        services.AddSingleton<PaymentMethodsRepository>(sp =>
        {
            var fibInstanceService = sp.GetRequiredService<IFibInstance>();
            return new PaymentMethodsRepository(fibInstanceService, FirebaseConstants.PAYMENT_METHODS_COLLECTION);
        });

        services.AddSingleton<IPaymentMethodsRepository>(sp =>
        {
            var inner = sp.GetRequiredService<PaymentMethodsRepository>();
            var cacheService = sp.GetRequiredService<ICatalogCacheService>();
            var metadataService = sp.GetRequiredService<ICatalogMetadataService>();
            return new CachedPaymentMethodsRepository(inner, cacheService, metadataService);
        });

        // Use cases
        services.AddTransient<IGetTransactionsUseCase, GetTransactionsUseCase>();
        services.AddTransient<ICreateTransactionUseCase, CreateTransactionUseCase>();
        services.AddTransient<IDeleteTransactionUseCase, DeleteTransactionUseCase>();
        services.AddTransient<IGetPaymentMethodsUseCase, GetPaymentMethodsUseCase>();
        services.AddTransient<IGetTagsUseCase, GetAllTagsUseCase>();
        services.AddTransient<IGetTransactionCatalogsUseCase, GetTransactionCatalogsUseCase>();

        return services;
    }
}
