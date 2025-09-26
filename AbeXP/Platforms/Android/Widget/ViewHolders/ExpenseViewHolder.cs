using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Plugins;
using Android.App;
using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using View = Android.Views.View;


namespace AbeXP.Platforms.Android.Widget.ViewHolders
{
    internal class ExpenseViewHolder : ExpenditureBaseViewHolder
    {

        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserSession _userSession;
        private DateTime expenseDate = DateTime.Now;
        

        public ExpenseViewHolder(View itemView, IExpenseRepository expenseRepository, IUserSession userSession) : base(itemView)
        {
            // TODO: we should create a single Use Case and inject it instead, to create the expense
            _expenseRepository = expenseRepository;
            _userSession = userSession;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            SetupDatePicker();
            SetupPaymentType();
            SetupTags();
            SetupSaveButton();
        }

        private void SetupPaymentType()
        {
            var ddlPaymentType = ItemView.FindViewById<MaterialAutoCompleteTextView>(Resource.Id.ddlPaymentType);

            string[] items = new[] { "Card", "Cash", "Transfer" };

            var adapter = new ArrayAdapter(
                ItemView.Context,
                global::Android.Resource.Layout.SimpleDropDownItem1Line,
                items
            );

            ddlPaymentType.Adapter = adapter;
            ddlPaymentType.SetText(items[0], false);
        }

        private void SetupTags()
        {
            var edtTags = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtTags);

            string[] tagOptions = { "Restaurant", "Business", "Travel", "Shopping" };
            bool[] selected = new bool[tagOptions.Length]; // track checked state

            edtTags.Click += (s, e) =>
            {
                new AlertDialog.Builder(ItemView.Context)
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
        }


        private void SetupSaveButton()
        {
            var btnSave = ItemView.FindViewById<MaterialButton>(Resource.Id.btnSaveExpense);
            btnSave.Click -= BtnSave_Click;
            btnSave.Click += BtnSave_Click;

            // store position if needed
            async void BtnSave_Click(object sender, EventArgs e)
            {

                try
                {
                    var expense = MapExpense();
                    expense.UserId = _userSession.UserId;  // Todo: this could be avoided here and set it in the User case to be created
                    var expenseIndexed = new ExpenseIndexed(expense);
                    await _expenseRepository.AddAsync(expenseIndexed);

                    NotifyWidgetUpdate();
                    Toast.MakeText(ItemView.Context, "Success", ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ItemView.Context, "Error while processing request", ToastLength.Short).Show();
                }
                finally
                {
                    await Task.Delay(1000);
                    FinishActivity();
                }

            }
        }


        private void SetupDatePicker()
        {
            var edtExpenseDate = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtExpenseDate);
            edtExpenseDate.Text = expenseDate.ToString(DateConstants.WidgetDateFormat);
            edtExpenseDate.Click += (s, e) =>
            {
                ShowDatePicker(ItemView.Context, edtExpenseDate, (date) => expenseDate = date);
            };

        }

        private Expense MapExpense()
        {
            var expense = new Expense();

            var edtDate = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtExpenseDate);
            var edtAmount = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtExpenseAmount);
            var ddlPaymentType = ItemView.FindViewById<MaterialAutoCompleteTextView>(Resource.Id.ddlPaymentType);
            var edtDescription = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtExpenseDescription);
            var edtTags = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtTags);


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

            expense.Date = expenseDate;
            return expense;
        }
    }
}
