using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.Platforms.Android.Widget.ViewHolders;
using Android.App;
using Android.Content;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.App.DatePickerDialog;
using View = Android.Views.View;


namespace AbeXP.Platforms.Android.Widget
{
    internal class ExpenseViewHolder : ExpenditureBaseViewHolder
    {

        private Expense expense = new Expense();
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseViewHolder(View itemView, IExpenseRepository expenseRepository) : base(itemView)
        {
            _expenseRepository = expenseRepository;

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
                    MapExpense(ItemView);
                    await _expenseRepository.AddAsync(expense);

                    Toast.MakeText(ItemView.Context, "Success", ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ItemView.Context, "Error while processing request", ToastLength.Short).Show();
                }
                finally
                {
                    await Task.Delay(2000);
                    FinishActivity();
                }

            }
        }


        private void SetupDatePicker()
        {
            var edtExpenseDate = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtExpenseDate);
            edtExpenseDate.Click += (s, e) =>
            {
                ShowDatePicker(ItemView.Context, edtExpenseDate, (date) => expense.Date = date);
            };

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
    }
}
