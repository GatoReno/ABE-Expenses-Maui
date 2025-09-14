using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using AbeXP.Abstractions.Interfaces;

namespace AbeXP.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IFibAuthLog _authService;

    public LoginViewModel(IFibAuthLog authService)
    {
        _authService = authService;
    }

    [ObservableProperty] private string email;
    [ObservableProperty] private string password;
    [ObservableProperty] private string confirmPassword;
    [ObservableProperty] private string userName;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage;
    [ObservableProperty] private bool isLoginMode = true;

    // Propiedades calculadas
    public string TitleText => IsLoginMode ? "Inicia Sesión" : "Crea tu cuenta";
    public string PrimaryButtonText => IsLoginMode ? "Iniciar sesión" : "Registrarme";
    public string SwitchButtonText => IsLoginMode ? "¿No tienes cuenta? Regístrate" : "¿Ya tienes cuenta? Inicia sesión";
    public bool IsSignUpMode => !IsLoginMode;
    public bool IsNotBusy => !IsBusy;

    // 🔹 Comando principal (Login o Registro)
    [RelayCommand]
    private async Task ExecutePrimaryActionAsync()
    {
        if (IsSignUpMode)
            await SignUpAsync();
        else
            await LoginAsync();
    }

    // 🔹 Login
    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var token = await _authService.SignInWithEmailAndPass(Email, Password);

            if (!string.IsNullOrEmpty(token))
            {
                Preferences.Set("firebase_token", token);
                // Navegación a home o siguiente pantalla
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // 🔹 Registro
    private async Task SignUpAsync()
    {
        try
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Las contraseñas no coinciden";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            var token = await _authService.CreateUserWithEmailAndPass(Email, Password);

            if (!string.IsNullOrEmpty(token))
            {
                Preferences.Set("firebase_token", token);
                // Navegación a home o siguiente pantalla
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // 🔹 Recuperar contraseña (simulada)
    [RelayCommand]
    private async Task RecoverPasswordAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Ingresa tu email para recuperar contraseña.";
                return;
            }

            IsBusy = true;
            await Task.Delay(1000); // simulación
            ErrorMessage = "Si existe la cuenta, se enviará un email de recuperación.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    // 🔹 Cambiar modo Login/SignUp
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
