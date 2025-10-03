using AbeXP.Interfaces;
using AbeXP.Platforms.Android.Widget.Service;
using Android.Appwidget;
using Android.Content;


[assembly: Dependency(typeof(WidgetUpdater))]
namespace AbeXP.Platforms.Android.Widget.Service
{
    public class WidgetUpdater : IWidgetUpdater
    {
        public void NotifyDataChanged()
        {
            var context = global::Android.App.Application.Context;
            var appWidgetManager = AppWidgetManager.GetInstance(context);

            var componentName = new ComponentName(context, Java.Lang.Class.FromType(typeof(QuickExpenseWidgetProvider)));
            var appWidgetIds = appWidgetManager.GetAppWidgetIds(componentName);

            appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds, Resource.Id.quickexpense_list);
        }

        public void Redraw()
        {
            var context = global::Android.App.Application.Context;
            var appWidgetManager = AppWidgetManager.GetInstance(context);

            var componentName = new ComponentName(context, Java.Lang.Class.FromType(typeof(QuickExpenseWidgetProvider)));
            var appWidgetIds = appWidgetManager.GetAppWidgetIds(componentName);

            var intent = new Intent(context, typeof(QuickExpenseWidgetProvider));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);
            context.SendBroadcast(intent);
        }
    }
}
