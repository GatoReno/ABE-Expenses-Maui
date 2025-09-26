using Android.App;
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
                editText.Text = args.Date.ToString("yyyy-MM-dd");
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
            if (ItemView.Context is Activity ac)
            {
                ac.Finish();
            }
        }

    }
}
