using AbeXP.Views;

namespace AbeXP;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(ExpenseFormView), typeof(ExpenseFormView));
        Routing.RegisterRoute(nameof(LoanFormView), typeof(LoanFormView));
        Routing.RegisterRoute(nameof(TransactionListPage), typeof(TransactionListPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

    }
}

