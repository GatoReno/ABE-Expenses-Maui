using AbeXP.Abstractions.Interfaces;
using AbeXP.Abstractions.Services;
using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Services;
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
        services.AddTransient<LoanFormViewModel>();
        services.AddSingleton<FinantialChartsViewModel>();
        services.AddTransient<AppShellViewModel>();


        // Views
        services.AddTransient<LoginView>();
        services.AddSingleton<MainPage>();
        services.AddTransient<ExpenseFormView>();
        services.AddTransient<IncomeFormView>();
        services.AddSingleton<FinantialChartsPage>();
        services.AddSingleton<LoanFormView>();
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
        services.AddSingleton<IExpenseRepository, ExpenseRepository>(sp =>
        {
            var fibInstanceService = sp.GetRequiredService<IFibInstance>();
            return new ExpenseRepository(fibInstanceService, FirebaseConstants.EXPENSES_COLLECTION);
        });

        services.AddSingleton<IIncomeRepository, IncomeRepository>(sp =>
        {
            var fibInstanceService = sp.GetRequiredService<IFibInstance>();
            return new IncomeRepository(fibInstanceService, FirebaseConstants.INCOMES_COLLECTION);
        });

        services.AddSingleton<ILoanRepository, LoanRepository>(sp =>
        {
            var fibInstanceService = sp.GetRequiredService<IFibInstance>();
            return new LoanRepository(fibInstanceService, FirebaseConstants.LOANS_COLLECTION);
        });

        services.AddSingleton<ITagsRepository, TagsRepository>(sp =>
        {
            var fibInstanceService = sp.GetRequiredService<IFibInstance>();
            return new TagsRepository(fibInstanceService, FirebaseConstants.TAGS_COLLECTION);
        });

        services.AddSingleton<IPaymentMethodsRepository, PaymentMethodsRepository>(sp =>
        {
            var fibInstanceService = sp.GetRequiredService<IFibInstance>();
            return new PaymentMethodsRepository(fibInstanceService, FirebaseConstants.PAYMENT_METHODS_COLLECTION);
        });

        // Use cases
        services.AddTransient<IGetTransactionsUseCase, GetTransactionsUseCase>();
        services.AddTransient<ICreateExpenseUseCase, CreateExpenseUseCase>();
        services.AddTransient<ICreateIncomeUseCase, CreateIncomeUseCase>();
        services.AddTransient<IGetPaymentMethodsUseCase, GetPaymentMethodsUseCase>();
        services.AddTransient<IGetTagsUseCase, GetAllTagsUseCase>();
        services.AddTransient<IGetTransactionCatalogsUseCase, GetTransactionCatalogsUseCase>();
        services.AddTransient<ICreateLoanUseCase, CreateLoanUseCase>();

        return services;
    }
}