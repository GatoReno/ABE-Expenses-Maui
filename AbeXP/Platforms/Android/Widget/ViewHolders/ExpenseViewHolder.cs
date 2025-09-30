using AbeXP.Common.Constants;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.Resources.Strings;
using AbeXP.UseCases.Interfaces;
using Android.App;
using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using View = Android.Views.View;


namespace AbeXP.Platforms.Android.Widget.ViewHolders
{
    internal class ExpenseViewHolder : ExpenditureBaseViewHolder
    {

        private readonly ICreateExpenseUseCase _createExpenseUse;
        private readonly IGetTransactionCatalogsUseCase _getTransactionCatalogsUseCase;
        private DateTime expenseDate = DateTime.Now;
        private PaymentMethodModelItem[] paymentMethods;
        private PaymentMethodModelItem selectedPaymentMethod;
        private TagModelItem[] tags;
        private bool[] checkedTags;
        private List<TagModelItem> selectedTags;

        public ExpenseViewHolder(View itemView, ICreateExpenseUseCase createExpenseUse, IGetTransactionCatalogsUseCase getTransactionCatalogsUseCase, IWidgetUpdater widgetUpdater) : base(itemView, widgetUpdater)
        {
            _createExpenseUse = createExpenseUse;
            _getTransactionCatalogsUseCase = getTransactionCatalogsUseCase;

            InitializeAsync();
        }


        private async void InitializeAsync()
        {
            await LoadData();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            SetupLocalization();
            SetupDatePicker();
            SetupPaymentType();
            SetupTags();
            SetupSaveButton();
        }

        private void SetupLocalization()
        {
            var edtDescription = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytExpenseDescription);
            edtDescription.Hint = AppResources.Description;

            var edtDate = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytExpenseDate);
            edtDate.Hint = AppResources.Date;

            var edtAmount = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytExpenseAmount);
            edtAmount.Hint = AppResources.Amount;

            var ddlPaymentTypeLayout = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytPaymentType);
            ddlPaymentTypeLayout.Hint = AppResources.PaymentMethod;

            var edtTagsLayout = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytTags);
            edtTagsLayout.Hint = AppResources.Tags;

            var btnSave = ItemView.FindViewById<MaterialButton>(Resource.Id.btnSaveExpense);
            btnSave.Text = AppResources.Save;
        }

        private void SetupPaymentType()
        {
            var ddlPaymentType = ItemView.FindViewById<MaterialAutoCompleteTextView>(Resource.Id.ddlPaymentType);
            var adapter = new ArrayAdapter(
                ItemView.Context,
                global::Android.Resource.Layout.SimpleDropDownItem1Line,
                paymentMethods
            );

            ddlPaymentType.Adapter = adapter;
            ddlPaymentType.SetText(paymentMethods[0].Name, false);
            ddlPaymentType.ItemClick += (s, e) =>
            {
                selectedPaymentMethod = paymentMethods[e.Position];
            };
        }

        private void SetupTags()
        {
            var edtTags = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtTags);

            var tagStringArray = tags.Select(tag => tag.Name).ToArray();
            checkedTags = tags.Select(tag => tag.IsSelected).ToArray();
            edtTags.Click += (s, e) =>
            {
                new AlertDialog.Builder(ItemView.Context)
                    .SetTitle("Select Tags")
                    .SetMultiChoiceItems(tagStringArray, checkedTags, (sender, args) =>
                    {
                        checkedTags[args.Which] = args.IsChecked;
                    })
                    .SetPositiveButton("OK", (sender, args) =>
                    {
                        var chosen = tags
                            .Where((t, i) => checkedTags[i]);

                        selectedTags = chosen.ToList();
                        edtTags.Text = string.Join(", ", chosen.Select(tag => tag.Name));
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
                    await _createExpenseUse.ExecuteAsync(expense);

                    NotifyWidgetUpdate();
                    Toast.MakeText(ItemView.Context, AppResources.Success, ToastLength.Short).Show();

                    ResetForm();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ItemView.Context, AppResources.ErrorWhileProcessingRequest, ToastLength.Short).Show();
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

        private void ResetForm()
        {
            expenseDate = DateTime.Now;
            selectedPaymentMethod = default;
            selectedTags = new List<TagModelItem>();
            if (checkedTags != null)
            {
                for (int i = 0; i < checkedTags.Length; i++)
                {
                    checkedTags[i] = false;
                }
            }

            var edtDescription = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtExpenseDescription);
            var edtDate = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtExpenseDate);
            var edtAmount = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtExpenseAmount);
            var ddlPaymentType = ItemView.FindViewById<MaterialAutoCompleteTextView>(Resource.Id.ddlPaymentType);
            var edtTags = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtTags);

            edtDescription.Text = string.Empty;
            edtDate.Text = expenseDate.ToString(DateConstants.WidgetDateFormat);
            edtAmount.Text = string.Empty;
            ddlPaymentType.SetText(string.Empty, false);
            edtTags.Text = string.Empty;
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
            expense.PaymentTypeId = selectedPaymentMethod?.Id;

            // Description
            expense.Description = edtDescription.Text ?? "";

            // Tags (assuming comma-separated in the EditText)
            expense.TagIds = selectedTags.Select(tag => tag.Id).ToList();

            expense.Date = expenseDate;
            return expense;
        }


        private async Task LoadData()
        {
            try
            {
                var itemsResult = await _getTransactionCatalogsUseCase.ExecuteAsync();
                if (itemsResult.IsSuccessful)
                {
                    paymentMethods = itemsResult.Payload.PaymentMethods.ToPaymentMethodItemList().ToArray();
                    tags = itemsResult.Payload.Tags.ToTagModelItemList().ToArray();
                }
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not load catalogs.");
            }
        }

    }
}
