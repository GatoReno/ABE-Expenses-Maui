using System.Collections;

namespace AbeXP.Controls;

public partial class PickerWithIcon : ContentView
{
    public PickerWithIcon()
    {
        InitializeComponent();
    }


    // ItemsSource
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList),
            typeof(PickerWithIcon),
            default(IList));

    public IList ItemsSource
    {
        get => (IList)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    // SelectedItem
    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(PickerWithIcon),
            default(object),
            BindingMode.TwoWay);

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    // Title
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(PickerWithIcon),
            string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }


    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Picker.IsFocused)
                Picker.Unfocus();

            Picker.Focus();
        });

    }
}