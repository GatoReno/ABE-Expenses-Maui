using AbeXP.Abstractions.Interfaces;
using AbeXP;
using AbeXP.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using AbeXP.Views;

namespace AbeXP.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly IFibAuthLog _fibAuthLog;
        private readonly IWidgetUpdater _widgetUpdater;
        private readonly INavigationService _navigationService;

        public AppShellViewModel(IFibAuthLog fibAuthLog, IWidgetUpdater widgetUpdater, INavigationService navigationService)
        {
            _fibAuthLog = fibAuthLog;
            _widgetUpdater = widgetUpdater;
            _navigationService = navigationService;
        }


        [RelayCommand]
        private async Task LogOut()
        {
            try
            {
                var confirmed = await App.Alert.ShowConfirmationAsync("Confirmación", "¿Deseas cerrar sesión?", "Sí", "No");
                if (!confirmed)
                    return;

                await _fibAuthLog.Logout();
                _widgetUpdater.Redraw();
                App.Instance.SetLoginPage();
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", $"Could not log out: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task NavigateProfileAsync()
        {
            try
            {
                await _navigationService.NavigateToAsync(nameof(ProfilePage));
                Shell.Current.FlyoutIsPresented = false;
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", $"Unable to open profile: {ex.Message}");
            }
        }
    }
}
