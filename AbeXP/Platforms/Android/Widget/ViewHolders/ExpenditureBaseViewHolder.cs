using AbeXP.Common.Constants;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using static Android.App.DatePickerDialog;
using View = Android.Views.View;

namespace AbeXP.Platforms.Android.Widget.ViewHolders
{
    internal class ExpenditureBaseViewHolder : RecyclerView.ViewHolder
    {
        public ExpenditureBaseViewHolder(View itemView) : base(itemView)
        {

        }


        /// <summary>
        /// Show a datepicker and pass the selected date to an Action
        /// </summary>
        /// <param name="context"></param>
        /// <param name="editText">to show the date as text</param>
        /// <param name="onDateChanged">action to do something with the new date</param>
        protected void ShowDatePicker(Context context, EditText editText, Action<DateTime> onDateChanged)
        {
            var today = DateTime.Today;

            void OnDatePickerChange(object? sender, DateSetEventArgs args)
            {
                editText.Text = args.Date.ToString(DateConstants.IndexDateFormat);
                onDateChanged?.Invoke(args.Date);
            }

            var dialog = new DatePickerDialog(context, OnDatePickerChange,
                today.Year, today.Month - 1, today.Day);
            dialog.Show();
        }

        /// <summary>
        /// Closes the pop up
        /// </summary>
        protected void FinishActivity()
        {
            var activity = GetActivity();
            if (activity != null)
            {
                activity.Finish();
            }
        }


        /// <summary>
        /// Triggers the widget to update the list of items
        /// </summary>
        protected void NotifyWidgetUpdate()
        {
            var activity = GetActivity();
            var appWidgetManager = AppWidgetManager.GetInstance(activity);

            var componentName = new ComponentName(activity, Java.Lang.Class.FromType(typeof(QuickExpenseWidgetProvider)));
            var appWidgetIds = appWidgetManager.GetAppWidgetIds(componentName);

            //var views = new RemoteViews(activity.PackageName, Resource.Layout.quickexpense_widget_layout);
            //appWidgetManager.UpdateAppWidget(appWidgetIds, views); // views is a RemoteViews that you need to build
            appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds, Resource.Id.quickexpense_list);
        }

        protected Activity GetActivity()
        {
            var context = ItemView.Context;

            while (context is ContextWrapper wrapper)
            {
                if (wrapper is Activity activity)
                    return activity;

                context = wrapper.BaseContext;
            }

            return default;
        }

    }
}
