using AbeXP.ViewModels;

namespace AbeXP;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

	 
}


