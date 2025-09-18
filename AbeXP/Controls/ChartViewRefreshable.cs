using Microcharts.Maui;
using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;

namespace AbeXP.Controls;

public class ChartViewRefreshable : ChartView
{
	public ChartViewRefreshable()
	{
		PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Listens for changes to the Chart property and forces a refresh of the chart view when it changes.
    /// ref https://github.com/microcharts-dotnet/Microcharts/issues/363 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Chart))
        {
            Dispatcher.Dispatch(() =>
            {
                WidthRequest = Width + 1;
                WidthRequest = Width - 1;
            });
        }
    }

    /// <summary>
    /// Unsubscribe from PropertyChanged event when the handler changes to avoid memory leaks.
    /// </summary>
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        // When the control is disposed, Handler will be null
        if (Handler == null)
        {
            this.PropertyChanged -= OnPropertyChanged;
        }
    }

}