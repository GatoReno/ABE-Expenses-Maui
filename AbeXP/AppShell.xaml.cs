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

        BindingContext = vm;
    }
}

