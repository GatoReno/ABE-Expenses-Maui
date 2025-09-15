namespace AbeXP.Controls;

public partial class DatePickerWithIcon : ContentView
{
    public DatePickerWithIcon()
    {
        InitializeComponent();
    }


    // Title bindable property
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(DatePickerWithIcon),
            default(string));

    // MaxAllowedDate bindable property
    public static readonly BindableProperty MaxDateAllowedProperty =
        BindableProperty.Create(
            nameof(MaxDateAllowed),
            typeof(DateTime),
            typeof(DatePickerWithIcon),
            DateTime.MaxValue);


    // MaxAllowedDate bindable property
    public static readonly BindableProperty MinDateAllowedProperty =
        BindableProperty.Create(
            nameof(MinDateAllowed),
            typeof(DateTime),
            typeof(DatePickerWithIcon),
            DateTime.MaxValue);


    // Date bindable property
    public static readonly BindableProperty DateProperty =
        BindableProperty.Create(
            nameof(Date),
            typeof(DateTime),
            typeof(DatePickerWithIcon),
            DateTime.Now,
            BindingMode.TwoWay);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public DateTime Date
    {
        get => (DateTime)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public DateTime MaxDateAllowed
    {
        get => (DateTime)GetValue(MaxDateAllowedProperty);
        set => SetValue(MaxDateAllowedProperty, value);
    }

    public DateTime MinDateAllowed
    {
        get => (DateTime)GetValue(MinDateAllowedProperty);
        set => SetValue(MinDateAllowedProperty, value);
    }
}
