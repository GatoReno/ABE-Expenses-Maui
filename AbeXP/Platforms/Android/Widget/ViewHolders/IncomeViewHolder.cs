using AbeXP.Common.Constants;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.Resources.Strings;
using AbeXP.UseCases.Interfaces;
using AbeXP.Common.Enum;
using Android.App;
using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using View = Android.Views.View;

namespace AbeXP.Platforms.Android.Widget.ViewHolders
{
    internal class IncomeViewHolder : ExpenditureBaseViewHolder
    {
        private readonly ICreateTransactionUseCase _createTransactionUseCase;
        private readonly IGetTransactionCatalogsUseCase _getTransactionCatalogsUseCase;
        private DateTime incomeDate = DateTime.Now;
        private PaymentMethodModelItem[] paymentMethods;
        private PaymentMethodModelItem selectedPaymentMethod;
        private TagModelItem[] tags;
        private bool[] checkedTags;
        private List<TagModelItem> selectedTags = new();

        public IncomeViewHolder(View itemView, ICreateTransactionUseCase createTransactionUseCase, IGetTransactionCatalogsUseCase getTransactionCatalogsUseCase, IWidgetUpdater widgetUpdater) : base(itemView, widgetUpdater)
        {
            _createTransactionUseCase = createTransactionUseCase;
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
            ItemView.FindViewById<TextInputLayout>(Resource.Id.lytIncomeDescription).Hint = AppResources.Description;
            ItemView.FindViewById<TextInputLayout>(Resource.Id.lytIncomeDate).Hint = AppResources.Date;
            ItemView.FindViewById<TextInputLayout>(Resource.Id.lytIncomeAmount).Hint = AppResources.Amount;
            ItemView.FindViewById<TextInputLayout>(Resource.Id.lytIncomePaymentType).Hint = AppResources.PaymentMethod;
            ItemView.FindViewById<TextInputLayout>(Resource.Id.lytIncomeTags).Hint = AppResources.Tags;
            ItemView.FindViewById<MaterialButton>(Resource.Id.btnSaveIncome).Text = AppResources.Save;
        }

        private void SetupPaymentType()
        {
            var ddlPaymentType = ItemView.FindViewById<MaterialAutoCompleteTextView>(Resource.Id.ddlIncomePaymentType);
            var adapter = new ArrayAdapter(
                ItemView.Context,
                global::Android.Resource.Layout.SimpleDropDownItem1Line,
                paymentMethods
            );
            ddlPaymentType.Adapter = adapter;
            if (paymentMethods.Length > 0)
            {
                ddlPaymentType.SetText(paymentMethods[0].Name, false);
                selectedPaymentMethod = paymentMethods[0];
            }
            ddlPaymentType.ItemClick += (s, e) => selectedPaymentMethod = paymentMethods[e.Position];
        }

        private void SetupTags()
        {
            var edtTags = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtIncomeTags);
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
                        var chosen = tags.Where((t, i) => checkedTags[i]);
                        selectedTags = chosen.ToList();
                        edtTags.Text = string.Join(", ", chosen.Select(tag => tag.Name));
                    })
                    .SetNegativeButton("Cancel", (sender, args) => { })
                    .Show();
            };
        }

        private void SetupSaveButton()
        {
            var btnSave = ItemView.FindViewById<MaterialButton>(Resource.Id.btnSaveIncome);
            btnSave.Click -= BtnSave_Click;
            btnSave.Click += BtnSave_Click;

            async void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    var transaction = MapIncomeTransaction();
                    await _createTransactionUseCase.ExecuteAsync(transaction);
                    NotifyWidgetUpdate();
                    Toast.MakeText(ItemView.Context, AppResources.Success, ToastLength.Short).Show();
                    ResetForm();
                }
                catch
                {
                    Toast.MakeText(ItemView.Context, AppResources.ErrorWhileProcessingRequest, ToastLength.Short).Show();
                }
            }
        }

        private void SetupDatePicker()
        {
            var edtIncomeDate = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtIncomeDate);
            edtIncomeDate.Text = incomeDate.ToString(DateConstants.WidgetDateFormat);
            edtIncomeDate.Click += (s, e) =>
            {
                ShowDatePicker(ItemView.Context, edtIncomeDate, (date) => incomeDate = date);
            };
        }

        private void ResetForm()
        {
            incomeDate = DateTime.Now;
            selectedPaymentMethod = default;
            selectedTags = new List<TagModelItem>();
            if (checkedTags != null)
            {
                for (int i = 0; i < checkedTags.Length; i++) checkedTags[i] = false;
            }
            ItemView.FindViewById<TextInputEditText>(Resource.Id.txtIncomeDescription).Text = string.Empty;
            ItemView.FindViewById<TextInputEditText>(Resource.Id.edtIncomeDate).Text = incomeDate.ToString(DateConstants.WidgetDateFormat);
            ItemView.FindViewById<TextInputEditText>(Resource.Id.txtIncomeAmount).Text = string.Empty;
            ItemView.FindViewById<MaterialAutoCompleteTextView>(Resource.Id.ddlIncomePaymentType).SetText(string.Empty, false);
            ItemView.FindViewById<TextInputEditText>(Resource.Id.edtIncomeTags).Text = string.Empty;
        }

        private TransactionModel MapIncomeTransaction()
        {
            var income = new TransactionModel { Type = TransactionType.Income };
            var edtAmount = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtIncomeAmount);
            var edtDescription = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtIncomeDescription);

            if (decimal.TryParse(edtAmount.Text, out var amount)) income.Amount = amount;
            income.PaymentTypeId = selectedPaymentMethod?.Id;
            income.Description = edtDescription.Text ?? string.Empty;
            income.TagIds = selectedTags.Select(t => t.Id).ToList();
            income.Date = incomeDate;
            return income;
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
            catch
            {
                App.Alert.ShowAlert("Error", "Could not load catalogs.");
            }
        }
    }
}
