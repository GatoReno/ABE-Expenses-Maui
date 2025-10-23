using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AbeXP.Abstractions.Interfaces;
using AbeXP.Interfaces;

namespace AbeXP.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IFibAuthLog _authService;
    private readonly IWidgetUpdater _widgetUpdater;
    private readonly INavigationService _navigationService;
    private readonly IAnalyticsService _analyticsService;

    public LoginViewModel(
        IFibAuthLog authService,
        IWidgetUpdater widgetUpdater,
        INavigationService navigationService,
        IAnalyticsService analyticsService)
    {
        _authService = authService;
        _widgetUpdater = widgetUpdater;
        _navigationService = navigationService;
        _analyticsService = analyticsService;
    }

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isLoginMode = true;

    public string TitleText => IsLoginMode ? "Inicia Sesión" : "Crea tu cuenta";
    public string PrimaryButtonText => IsLoginMode ? "Iniciar sesión" : "Registrarme";
    public string SwitchButtonText => IsLoginMode ? "¿No tienes cuenta? Regístrate" : "¿Ya tienes cuenta? Inicia sesión";
    public bool IsSignUpMode => !IsLoginMode;
    public bool IsNotBusy => !IsBusy;

    [RelayCommand]
    private async Task ExecutePrimaryActionAsync()
    {
        if (IsSignUpMode)
            await SignUpAsync();
        else
            await LoginAsync();
    }

    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await _analyticsService.LogEventAsync("login_attempt", new Dictionary<string, string>
            {
                ["method"] = "email_password",
                ["has_email"] = (!string.IsNullOrWhiteSpace(Email)).ToString().ToLowerInvariant()
            });

            await _authService.SignInWithEmailAndPass(Email, Password);

            await _analyticsService.LogEventAsync("login_success", new Dictionary<string, string>
            {
                ["method"] = "email_password"
            });

            _widgetUpdater.Redraw();
            App.Instance.SetNewShellPage();
        }
        catch (Exception ex)
        {
            await _analyticsService.LogEventAsync("login_failure", new Dictionary<string, string>
            {
                ["method"] = "email_password",
                ["error"] = ex.Message
            });

            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SignUpAsync()
    {
        try
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Las contraseñas no coinciden";
                await _analyticsService.LogEventAsync("signup_validation_failed", new Dictionary<string, string>
                {
                    ["reason"] = "password_mismatch"
                });
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            await _analyticsService.LogEventAsync("signup_attempt", new Dictionary<string, string>
            {
                ["method"] = "email_password",
                ["has_email"] = (!string.IsNullOrWhiteSpace(Email)).ToString().ToLowerInvariant()
            });

            await _authService.CreateUserWithEmailAndPass(Email, Password);

            await _analyticsService.LogEventAsync("signup_success", new Dictionary<string, string>
            {
                ["method"] = "email_password"
            });

            _widgetUpdater.Redraw();
            App.Instance.SetNewShellPage();
        }
        catch (Exception ex)
        {
            await _analyticsService.LogEventAsync("signup_failure", new Dictionary<string, string>
            {
                ["method"] = "email_password",
                ["error"] = ex.Message
            });

            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RecoverPasswordAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Ingresa tu email para recuperar contraseña.";
                await _analyticsService.LogEventAsync("password_recovery_failed", new Dictionary<string, string>
                {
                    ["reason"] = "missing_email"
                });
                return;
            }

            IsBusy = true;
            await Task.Delay(1000);
            ErrorMessage = "Si existe la cuenta, se enviará un email de recuperación.";

            await _analyticsService.LogEventAsync("password_recovery_requested", new Dictionary<string, string>
            {
                ["status"] = "simulated"
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await _analyticsService.LogEventAsync("password_recovery_failed", new Dictionary<string, string>
            {
                ["reason"] = "exception",
                ["error"] = ex.Message
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SwitchMode()
    {
        IsLoginMode = !IsLoginMode;
        OnPropertyChanged(nameof(TitleText));
        OnPropertyChanged(nameof(PrimaryButtonText));
        OnPropertyChanged(nameof(SwitchButtonText));
        OnPropertyChanged(nameof(IsSignUpMode));
    }
}
