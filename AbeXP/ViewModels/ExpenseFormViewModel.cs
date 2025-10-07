using AbeXP.Models;
using AbeXP.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AbeXP.UseCases.Interfaces;
using AbeXP.Extensions;
using AbeXP.Common.Constants;
using AbeXP.Common.Enum;

namespace AbeXP.ViewModels
{
    public partial class ExpenseFormViewModel : ObservableObject
    {
        private readonly IGetTransactionCatalogsUseCase _getTransactionCatalogsUseCase;
        private readonly ICreateTransactionUseCase _createTransactionUseCase;
        private readonly INavigationService _navigation;

        public ExpenseFormViewModel(INavigationService navigation, ICreateTransactionUseCase createTransactionUseCase, IGetTransactionCatalogsUseCase getTransactionCatalogsUseCase)
        {
            _navigation = navigation;
            _createTransactionUseCase = createTransactionUseCase;
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
        private PaymentMethod selectedPaymentType;

        [ObservableProperty]
        public ObservableCollection<TagModelItem> _tags;

        [ObservableProperty]
        public ObservableCollection<PaymentMethod> _paymentMethods;

        [ObservableProperty]
        public bool _isBusy;
        #endregion


        [RelayCommand]
        private async Task SaveExpense()
        {
            IsBusy = true;
            try
            {
                var transaction = new ExpenseTransactionModel
                {
                    Type = TransactionType.Expense,
                    Date = Date,
                    Amount = Amount,
                    Description = Description,
                    PaymentTypeId = SelectedPaymentType?.Id,
                    TagIds = Tags.Where(t => t.IsSelected).Select(t => t.Id).ToList()
                };

                var result = await _createTransactionUseCase.ExecuteAsync(transaction);
                if (result.IsSuccessful)
                {
                    App.Alert.ShowToast("Expense saved successfully.");

                    var routePramteres = new ShellNavigationQueryParameters()
                    {
                        { NavigationConstants.TRIGGER_DASHBOARD_PARAM, true }
                    };
                    await _navigation.PopAsync(routePramteres);
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
                    var paymentMethods = result.Payload.PaymentMethods;

                    Tags = new ObservableCollection<TagModelItem>(tagItems);
                    PaymentMethods = new ObservableCollection<PaymentMethod>(paymentMethods);
                }
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not load payment types and tags.");
            }
        }
    }
}
