using Microsoft.Maui.Handlers;
using System.ComponentModel;

namespace AbeXP.Controls;

public partial class DatePickerEntry : ContentView
{
	public DatePickerEntry()
	{
		InitializeComponent();
    }
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        DatePicker.Unfocus();
        DatePicker.Focus();

#if ANDROID
        var handler = DatePicker.Handler as IDatePickerHandler;
        handler.PlatformView.PerformClick();
#endif

#if IOS
        var iosHandler = DatePicker.Handler as IDatePickerHandler;
        iosHandler.PlatformView.ResignFirstResponder();
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