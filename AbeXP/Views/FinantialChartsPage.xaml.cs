using AbeXP.ViewModels;
using Android.SE.Omapi;
using System.ComponentModel;

namespace AbeXP.Views;

public partial class FinantialChartsPage : ContentPage
{
    public FinantialChartsPage(FinantialChartsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

        vm.PropertyChanged += RedrawCharts;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ((FinantialChartsViewModel)BindingContext).PropertyChanged -= RedrawCharts;
    }

    private void RedrawCharts(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FinantialChartsViewModel.ExpensesLineChart))
        {
            this.ExpensesLineChart.WidthRequest = this.ExpensesLineChart.Width + 1;
            this.ExpensesLineChart.WidthRequest = this.ExpensesLineChart.Width - 1;
        }
    }
}