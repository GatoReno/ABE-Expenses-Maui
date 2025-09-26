using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

namespace AbeXP.Platforms.Android
{
    [BroadcastReceiver(Label = "Quick Expenses")]
    [IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
    [MetaData("android.appwidget.provider", Resource = "@xml/my_widget_provider")]
    public class MyWidgetProvider : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            foreach (int appWidgetId in appWidgetIds)
            {
                var intent = new Intent(context, typeof(AddItemActivity));
                var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.Immutable);

                var views = new RemoteViews(context.PackageName, Resource.Layout.widget_layout);
                views.SetOnClickPendingIntent(Resource.Id.widget_add_button, pendingIntent);

                appWidgetManager.UpdateAppWidget(appWidgetId, views);
            }
        }
    }
}
