using AbeXP.Views;

namespace AbeXP;

public partial class AppShell : Shell
{
	public AppShell(IServiceProvider sp)
	{
        InitializeComponent();
        Routing.RegisterRoute(nameof(ExpenseFormView), typeof(ExpenseFormView));
        Routing.RegisterRoute(nameof(LoanFormView), typeof(LoanFormView));
        Routing.RegisterRoute(nameof(TransactionListPage), typeof(TransactionListPage));

    }
}

