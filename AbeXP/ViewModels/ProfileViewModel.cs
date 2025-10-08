using AbeXP.UseCases.Plugins;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AbeXP.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IUserSession _userSession;

    public ProfileViewModel(IUserSession userSession)
    {
        _userSession = userSession;
        LoadUserInfo();
    }

    [ObservableProperty]
    private string email = string.Empty;

    private void LoadUserInfo()
    {
        Email = _userSession?.User?.Email ?? string.Empty;
    }
}
