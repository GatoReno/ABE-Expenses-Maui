using AbeXP.Models;
using AbeXP.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AbeXP.UseCases.Interfaces;
using AbeXP.Extensions;

namespace AbeXP.ViewModels
{
    public partial class ExpenseFormViewModel : ObservableObject
    {
        private readonly IGetTransactionCatalogsUseCase _getTransactionCatalogsUseCase;
        private readonly ICreateExpenseUseCase _createExpenseUseCase;
        private readonly INavigationService _navigation;

        public ExpenseFormViewModel(INavigationService navigation, ICreateExpenseUseCase createExpenseUseCase, IGetTransactionCatalogsUseCase getTransactionCatalogsUseCase)
        {
            _navigation = navigation;
            _createExpenseUseCase = createExpenseUseCase;
            _getTransactionCatalogsUseCase = getTransactionCatalogsUseCase;

            GetCatalogs();
        }

        #region PROPERTIES
        [ObservableProperty]
        private DateTime date = DateTime.Now;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private PaymentMethodItem selectedPaymentType;

        [ObservableProperty]
        public ObservableCollection<TagModelItem> _tags;

        [ObservableProperty]
        public ObservableCollection<PaymentMethodItem> _paymentMethods;

        [ObservableProperty]
        public bool _isBusy;
        #endregion


        [RelayCommand]
        private async Task SaveExpense()
        {
            IsBusy = true;
            try
            {
                var expense = new Expense
                {
                    Date = Date,
                    Amount = Amount,
                    Description = Description,
                    PaymentTypeId = SelectedPaymentType?.Id,
                    TagIds = Tags.Where(t => t.IsSelected).Select(t => t.Id).ToList()
                };

                var result = await _createExpenseUseCase.ExecuteAsync(expense);
                if (result.IsSuccessful)
                {
                    App.Alert.ShowToast("Expense saved successfully.");
                    await _navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not save data.");
            }
            finally
            {
                IsBusy = false;
            }
        }


        /// <summary>
        /// Get the catalogs for the form
        /// </summary>
        /// <returns></returns>
        private async Task GetCatalogs()
        {
            try
            {
                var result = await _getTransactionCatalogsUseCase.ExecuteAsync();
                if (result.IsSuccessful)
                {
                    var tagItems = result.Payload.Tags.ToTagModelItemList();
                    var paymentMethods = result.Payload.PaymentMethods.ToPaymentMethodItemList();

                    Tags = new ObservableCollection<TagModelItem>(tagItems);
                    PaymentMethods = new ObservableCollection<PaymentMethodItem>(paymentMethods);
                }
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not load payment types and tags.");
            }
        }
    }
}
