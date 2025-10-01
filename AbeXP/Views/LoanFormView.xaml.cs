using AbeXP.ViewModels;

namespace AbeXP.Views;

public partial class LoanFormView : ContentPage
{
	public LoanFormView(LoanFormViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}
