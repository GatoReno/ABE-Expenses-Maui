using AbeXP.Interfaces;
using AbeXP.Models;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Adapter;
using Google.Android.Material.Button;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;
using System.Collections;
using static Android.App.DatePickerDialog;
using static AndroidX.RecyclerView.Widget.RecyclerView;
using View = Android.Views.View;

namespace AbeXP.Platforms.Android
{
    public class AddItemPagerAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        private readonly IExpenseRepository _expenseRepository;
        private readonly int[] _layouts;


        private Expense expense = new Expense();
        private Loan loan = new Loan();

        public AddItemPagerAdapter(Context c, IExpenseRepository expenseRepository)
        {
            _context = c;
            _expenseRepository = expenseRepository;
            _layouts = new[]
            {
                Resource.Layout.expense_tab,
                Resource.Layout.loan_tab
            };
        }

        public override int ItemCount => _layouts.Length;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // get the root view of this page
            var itemView = holder.ItemView;






            if (position == 0) // Expense tab SimpleDropDownItem1Line
            {

                // PaymentType
                var ddlPaymentType = itemView.FindViewById<MaterialAutoCompleteTextView>(Resource.Id.ddlPaymentType);

                string[] items = new[] { "Card", "Cash", "Transfer" };

                var adapter = new ArrayAdapter(
                    holder.ItemView.Context,
                    global::Android.Resource.Layout.SimpleDropDownItem1Line,
                    items
                );

                ddlPaymentType.Adapter = adapter;
                ddlPaymentType.SetText(items[0], false);



                //Tags
                var edtTags = holder.ItemView.FindViewById<TextInputEditText>(Resource.Id.edtTags);

                string[] tagOptions = { "Restaurant", "Business", "Travel", "Shopping" };
                bool[] selected = new bool[tagOptions.Length]; // track checked state

                edtTags.Click += (s, e) =>
                {
                    new AlertDialog.Builder(holder.ItemView.Context)
                        .SetTitle("Select Tags")
                        .SetMultiChoiceItems(tagOptions, selected, (sender, args) =>
                        {
                            selected[args.Which] = args.IsChecked;
                        })
                        .SetPositiveButton("OK", (sender, args) =>
                        {
                            var chosen = tagOptions
                                .Where((t, i) => selected[i])
                                .ToArray();

                            edtTags.Text = string.Join(", ", chosen);
                        })
                        .SetNegativeButton("Cancel", (sender, args) => { })
                        .Show();
                };


                //Date
                var edtExpenseDate = itemView.FindViewById<TextInputEditText>(Resource.Id.edtExpenseDate);
                edtExpenseDate.Click += (s, e) =>
                {
                    ShowDatePicker(itemView.Context, edtExpenseDate, (date) => expense.Date = date);
                };


                //save button
                var btnSave = itemView.FindViewById<MaterialButton>(Resource.Id.btnSaveExpense);
                btnSave.Click -= BtnSave_Click;
                btnSave.Click += BtnSave_Click;

                // store position if needed
                async void BtnSave_Click(object sender, EventArgs e)
                {

                    try
                    {
                        MapExpense(itemView);
                        await _expenseRepository.AddAsync(expense);

                        Toast.MakeText(_context, "Success", ToastLength.Short).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(_context, "Error while processing request", ToastLength.Short).Show();
                    }
                    finally
                    {
                        await Task.Delay(2000);
                        FinishActivity();
                    }

                }
            }
            else if (position == 1) // Loan tab
            {
                var edtLoanDate = itemView.FindViewById<EditText>(Resource.Id.edtLoanDate);
                var edtPaymentDate = itemView.FindViewById<EditText>(Resource.Id.edtPaymentDate);

                edtLoanDate.Click += (s, e) => ShowDatePicker(itemView.Context, edtLoanDate, (date) => loan.DateGiven = date);
                edtPaymentDate.Click += (s, e) => ShowDatePicker(itemView.Context, edtPaymentDate, (date) => loan.SuggestedPaybackDate = date);


                // save btn
                var btnSave = itemView.FindViewById<MaterialButton>(Resource.Id.btnSaveLoan);
                btnSave.Click -= BtnSave_Click;
                btnSave.Click += BtnSave_Click;

                // store position if needed
                void BtnSave_Click(object sender, EventArgs e)
                {
                    FinishActivity();
                }


            }
        }

        /// <summary>
        /// Show a datepicker and pass the selected date to an Action
        /// </summary>
        /// <param name="context"></param>
        /// <param name="editText">to show the date as text</param>
        /// <param name="onDateChanged">action to do something with the new date</param>
        private void ShowDatePicker(Context context, EditText editText, Action<DateTime> onDateChanged)
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

        private void MapExpense(View view)
        {
            var edtDate = view.FindViewById<TextInputEditText>(Resource.Id.edtExpenseDate);
            var edtAmount = view.FindViewById<TextInputEditText>(Resource.Id.txtExpenseAmount);
            var ddlPaymentType = view.FindViewById<MaterialAutoCompleteTextView>(Resource.Id.ddlPaymentType);
            var edtDescription = view.FindViewById<TextInputEditText>(Resource.Id.txtExpenseDescription);
            var edtTags = view.FindViewById<TextInputEditText>(Resource.Id.edtTags);


            // Amount
            if (decimal.TryParse(edtAmount.Text, out var amount))
            {
                expense.Amount = amount;
            }

            // Payment type
            expense.PaymentTypeId = ddlPaymentType.Text ?? "";

            // Description
            expense.Description = edtDescription.Text ?? "";

            // Tags (assuming comma-separated in the EditText)
            expense.TagIds = (edtTags.Text ?? "")
                                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                .ToList();

        }


        /// <summary>
        /// Closes the pop up
        /// </summary>
        private void FinishActivity()
        {
            if (_context is Activity ac)
            {
                ac.Finish();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var _inflater = LayoutInflater.From(_context);

            var view = _inflater.Inflate(_layouts[viewType], parent, false);
            return new SimpleViewHolder(view);
        }

        public override int GetItemViewType(int position) => position;

        class SimpleViewHolder : RecyclerView.ViewHolder
        {
            public SimpleViewHolder(View itemView) : base(itemView) { }
        }

    }
}
