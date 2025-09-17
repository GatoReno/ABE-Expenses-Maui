using System.Windows.Input;

namespace AbeXP.Controls;

public partial class ButtonWithContent : ContentView
{
    public ButtonWithContent()
    {
        InitializeComponent();
    }

    // Command bindable property
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(ButtonWithContent),
            default(string),
            propertyChanged: CommandPropertyChanged);

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    private static void CommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ButtonWithContent control)
            return;

        if (oldValue is ICommand oldCmd)
        {
            oldCmd.CanExecuteChanged -= control.OnCanExecuteChanged;
        }
        if (newValue is ICommand newCmd)
        {
            newCmd.CanExecuteChanged += control.OnCanExecuteChanged;
            //// force initial evaluation
            //control.OnCanExecuteChanged(newCmd, EventArgs.Empty);
        }
    }

    private void OnCanExecuteChanged(object sender, EventArgs e)
    {
        if (sender is ICommand cmd)
        {
            IsBusy = !cmd.CanExecute(null);
        }
    }



    // CanExecute bindable property
    public static readonly BindableProperty IsBusyProperty =
        BindableProperty.Create(
            nameof(IsBusy),
            typeof(bool),
            typeof(ButtonWithContent),
            default(bool));
    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    // Color bindable property
    public static new readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(ButtonWithContent),
            default(Color));

    public new Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }



}