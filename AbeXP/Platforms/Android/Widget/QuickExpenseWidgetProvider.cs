using AbeXP.Resources.Strings;
using AbeXP.UseCases.Plugins;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Views;
using Android.Widget;
using static AbeXP.Platforms.Android.Widget.Service.QuickExpenseListService;

namespace AbeXP.Platforms.Android.Widget
{
    [BroadcastReceiver(Label = "Quick Expenses")]
    [IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
    [MetaData("android.appwidget.provider", Resource = "@xml/quickexpense_widget_provider")]
    public class QuickExpenseWidgetProvider : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {

            var userSession = MauiApplication.Current.Services.GetService<IUserSession>();
            var views = new RemoteViews(context.PackageName, Resource.Layout.quickexpense_widget_layout);

            foreach (int appWidgetId in appWidgetIds)
            {

                // Set localized strings
                views.SetTextViewText(Resource.Id.txtQuickExpenseHeader, AppResources.QuickExpenses);
                views.SetTextViewText(Resource.Id.txtQuickExpenseEmpty, AppResources.NoExpenses);
                views.SetTextViewText(Resource.Id.txtQuickExpenseLoginRequired, AppResources.PleaseLoginExpenses);
                views.SetTextViewText(Resource.Id.btnQuickexpenseAdd, AppResources.AddExpenseLoan);


                if (userSession.IsLoggedIn)
                {
                    views.SetViewVisibility(Resource.Id.txtQuickExpenseLoginRequired, ViewStates.Gone);
                    views.SetViewVisibility(Resource.Id.quickexpense_list, ViewStates.Visible);
                    views.SetViewVisibility(Resource.Id.btnQuickexpenseAdd, ViewStates.Visible);

                    var intent = new Intent(context, typeof(WidgetListService));
                    intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);
                    intent.SetData(global::Android.Net.Uri.Parse(intent.ToUri(IntentUriType.Scheme)));

                    views.SetRemoteAdapter(Resource.Id.quickexpense_list, intent);
                    views.SetEmptyView(Resource.Id.quickexpense_list, Resource.Id.txtQuickExpenseEmpty);

                    // PendingIntent for Add button
                    var addIntent = new Intent(context, typeof(AddItemActivity));
                    var pendingIntent = PendingIntent.GetActivity(context, 0, addIntent, PendingIntentFlags.Immutable);
                    views.SetOnClickPendingIntent(Resource.Id.btnQuickexpenseAdd, pendingIntent);

                }
                else
                {
                    views.SetViewVisibility(Resource.Id.txtQuickExpenseLoginRequired, ViewStates.Visible);

                    views.SetViewVisibility(Resource.Id.txtQuickExpenseHeader, ViewStates.Visible);
                    views.SetViewVisibility(Resource.Id.quickexpense_list, ViewStates.Gone);
                    views.SetViewVisibility(Resource.Id.btnQuickexpenseAdd, ViewStates.Gone);
                }
                
                appWidgetManager.UpdateAppWidget(appWidgetId, views);
            }
        }
    }
}
