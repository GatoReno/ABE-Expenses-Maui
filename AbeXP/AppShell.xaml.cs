using AbeXP.ViewModels;
using AbeXP.Views;

namespace AbeXP;

public partial class AppShell : Shell
{
	public AppShell(AppShellViewModel vm)
	{
        InitializeComponent();
        Routing.RegisterRoute(nameof(ExpenseFormView), typeof(ExpenseFormView));
        Routing.RegisterRoute(nameof(IncomeFormView), typeof(IncomeFormView));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        BindingContext = vm;
    }
}
