using AbeXP.Interfaces;
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
            var expenseRepository = MauiApplication.Current.Services.GetService<IExpenseRepository>();
            var loanRepository = MauiApplication.Current.Services.GetService<ILoanRepository>();
            var userSession = MauiApplication.Current.Services.GetService<IUserSession>();

            // tab titles
            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            var viewPager = FindViewById<ViewPager2>(Resource.Id.viewPager);

            var adapter = new AddItemPagerAdapter(this, expenseRepository, loanRepository, userSession);
            viewPager.Adapter = adapter;

            var titles = new[] { "Expense", "Loan" };

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
