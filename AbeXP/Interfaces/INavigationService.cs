namespace AbeXP.Interfaces
{
    internal interface INavigationService
    {
        Task InitializeAsync();
        Task NavigateToAsync(string route, IDictionary<string, object> routeParameters = null);
        Task PopAsync();
    }
}
