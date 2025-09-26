using Android.App;
using Android.Appwidget;
using Android.Content;
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
            foreach (int appWidgetId in appWidgetIds)
            {
                var intent = new Intent(context, typeof(WidgetListService));
                intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);
                intent.SetData(global::Android.Net.Uri.Parse(intent.ToUri(IntentUriType.Scheme)));


                var views = new RemoteViews(context.PackageName, Resource.Layout.quickexpense_widget_layout);
                views.SetRemoteAdapter(Resource.Id.quickexpense_list, intent);

                // PendingIntent for Add button
                var addIntent = new Intent(context, typeof(AddItemActivity));
                var pendingIntent = PendingIntent.GetActivity(context, 0, addIntent, PendingIntentFlags.Immutable);
                views.SetOnClickPendingIntent(Resource.Id.quickexpense_add_button, pendingIntent);

                appWidgetManager.UpdateAppWidget(appWidgetId, views);
            }
        }
    }
}
