using AbeXP.ViewModels;

namespace AbeXP.Views;

public partial class IncomeFormView : ContentPage
{
    public IncomeFormView(IncomeFormViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
