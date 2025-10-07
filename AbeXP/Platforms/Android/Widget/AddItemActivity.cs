using AbeXP.Interfaces;
using AbeXP.Resources.Strings;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using Button = Android.Widget.Button;

namespace AbeXP.Platforms.Android.Widget
{
    [Activity(Theme = "@style/Theme.TransparentDialog",
    Exported = true,
    TaskAffinity = "",
    ExcludeFromRecents = true,
    LaunchMode = LaunchMode.SingleTask)]
    public class AddItemActivity : Activity
    {

        public AddItemActivity()
        {

        }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.dialog_add_item);

            SetupTabs();
        }

        private void SetupTabs()
        {
            // get repository from MAUI DI
            var createTransactionUseCase = MauiApplication.Current.Services.GetService<ICreateTransactionUseCase>();
            var getTransactionCatalogsUseCase = MauiApplication.Current.Services.GetService<IGetTransactionCatalogsUseCase>();
            var widgetUpdater = MauiApplication.Current.Services.GetService<IWidgetUpdater>();

            // tab titles
            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            var viewPager = FindViewById<ViewPager2>(Resource.Id.viewPager);

            var adapter = new AddItemPagerAdapter(this, createTransactionUseCase, getTransactionCatalogsUseCase, widgetUpdater);
            viewPager.Adapter = adapter;

            var titles = new[] { AppResources.Expense, AppResources.Income };

            // create and attach the mediator using a C# implementation of the strategy interface
            var mediator = new TabLayoutMediator(tabLayout, viewPager, new TabConfigStrategy(titles));
            mediator.Attach();
        }


    }

    // small class that implements the Java callback interface
    class TabConfigStrategy : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
    {
        private readonly string[] _titles;
        public TabConfigStrategy(string[] titles) => _titles = titles;

        // Called for each tab to configure its text/icon/etc.
        public void OnConfigureTab(TabLayout.Tab tab, int position)
        {
            tab.SetText(_titles[position]);
        }
    }
}
