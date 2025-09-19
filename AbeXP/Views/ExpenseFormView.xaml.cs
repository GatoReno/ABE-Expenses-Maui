using AbeXP.ViewModels;

namespace AbeXP.Views;

public partial class ExpenseFormView : ContentPage
{
	public ExpenseFormView(ExpenseFormViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
