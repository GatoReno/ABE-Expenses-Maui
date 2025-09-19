using AbeXP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Services
{
    internal class MauiNavigationService : INavigationService
    {
        private readonly ISettingsService _settingsService;

        public MauiNavigationService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public Task InitializeAsync() =>
            NavigateToAsync(
                string.IsNullOrEmpty(_settingsService.AuthAccessToken)
                    ? "//Login"
                    : "//Main");

        public Task NavigateToAsync(string route, IDictionary<string, object> routeParameters = null)
        {
            var shellNavigation = new ShellNavigationState(route);

            return routeParameters != null
                ? Shell.Current.GoToAsync(shellNavigation, routeParameters)
                : Shell.Current.GoToAsync(shellNavigation);
        }

        public Task PopAsync() =>
            Shell.Current.GoToAsync("..");
    }
}
