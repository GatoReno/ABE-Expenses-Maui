using AbeXP.ViewModels;

namespace AbeXP.Views;

public partial class FinantialChartsPage : ContentPage
{
    public FinantialChartsPage(FinantialChartsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
