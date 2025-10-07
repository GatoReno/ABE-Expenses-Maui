using Microsoft.Maui.Handlers;
using System.ComponentModel;
#if IOS
using UIKit;
using Microsoft.Maui.Handlers;
#endif
namespace AbeXP.Controls;

public partial class DatePickerEntry : ContentView
{
	public DatePickerEntry()
	{
		InitializeComponent();
    }
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        
#if ANDROID
        
        DatePicker.Unfocus();
        DatePicker.Focus();
        
        var handler = DatePicker.Handler as IDatePickerHandler;
        handler.PlatformView.PerformClick();
#endif

#if IOS
        var handler = DatePicker.Handler as DatePickerHandler;
        if (handler?.PlatformView is Microsoft.Maui.Platform.MauiDatePicker nativePicker)
        {
            // Forzar que se muestre el picker
            if (nativePicker.InputView is UIDatePicker uiDatePicker)
            {
                // Aquí tienes acceso al UIDatePicker nativo si lo necesitas
                uiDatePicker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
                
            }
            DatePicker.Focus();
        }
#endif
    }

    private void DatePicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
    }

    // Hint bindable property
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Hint),
            typeof(string),
            typeof(DatePickerEntry),
            default(string));

    // Date bindable property
    public static readonly BindableProperty DateProperty =
        BindableProperty.Create(
            nameof(Date),
            typeof(DateTime),
            typeof(DatePickerEntry),
            DateTime.Now,
            BindingMode.TwoWay);

    public string Hint
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public DateTime Date
    {
        get => (DateTime)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }


}