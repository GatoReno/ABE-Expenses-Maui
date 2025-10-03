using AbeXP.Abstractions.Interfaces;
using AbeXP.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly IFibAuthLog _fibAuthLog;
        private readonly IWidgetUpdater _widgetUpdater;

        public AppShellViewModel(IFibAuthLog fibAuthLog, IWidgetUpdater widgetUpdater)
        {
            _fibAuthLog = fibAuthLog;
            _widgetUpdater = widgetUpdater;
        }


        [RelayCommand]
        private void LogOut()
        {
            try
            {
                _fibAuthLog.Logout();
                _widgetUpdater.Redraw();
                App.Instance.SetLoginPage();
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", $"Could not log out: {ex.Message}");
            }
        }
    }
}
