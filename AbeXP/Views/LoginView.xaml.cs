using AbeXP.ViewModels;

namespace AbeXP.Views;

public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}
