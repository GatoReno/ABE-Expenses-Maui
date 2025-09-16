using AbeXP.ViewModels;
using Microcharts.Maui;
using System.ComponentModel;

namespace AbeXP.Views;

public partial class FinantialChartsPage : ContentPage
{
    public FinantialChartsPage(FinantialChartsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        vm.PropertyChanged += HandleChartsEntriesChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ((FinantialChartsViewModel)BindingContext).PropertyChanged -= HandleChartsEntriesChanged;
    }

    /// <summary>
    /// Handle the PropertyChanged event of the ViewModel to refresh the charts when their data changes.
    /// </summary>
    /// <param name="sender">Triggerer of the event</param>
    /// <param name="e">Property changed arguments</param>
    private void HandleChartsEntriesChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(FinantialChartsViewModel.ExpensesLineChart):
                RefreshChart(this.ExpensesLineChart);
                break;
            case nameof(FinantialChartsViewModel.PaymentsTypeDonutChart):
                RefreshChart(this.PaymentsTypeDonutChart);
                break;
            case nameof(FinantialChartsViewModel.TagsBarChart):
                RefreshChart(this.TagsBarChart);
                break;
        }
    }

    /// <summary>
    /// Forces the specified chart view to refresh its layout and appearance.
    /// Ref. https://github.com/microcharts-dotnet/Microcharts/issues/363
    /// </summary>
    /// <param name="chartView">The chart view to be refreshed. Cannot be null.</param>
    private void RefreshChart(ChartView chartView)
    {
        chartView.Dispatcher.Dispatch(() =>
        {
            chartView.WidthRequest = chartView.Width + 1;
            chartView.WidthRequest = chartView.Width - 1;
        });
    }

}