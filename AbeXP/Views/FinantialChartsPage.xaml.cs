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
    }
}