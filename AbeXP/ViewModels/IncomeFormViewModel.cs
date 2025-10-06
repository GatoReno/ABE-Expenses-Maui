using AbeXP.Models;
using AbeXP.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AbeXP.UseCases.Interfaces;
using AbeXP.Extensions;

namespace AbeXP.ViewModels
{
    public partial class IncomeFormViewModel : ObservableObject
    {
        private readonly IGetTransactionCatalogsUseCase _getTransactionCatalogsUseCase;
        private readonly ICreateIncomeUseCase _createIncomeUseCase;
        private readonly INavigationService _navigation;

        public IncomeFormViewModel(INavigationService navigation, ICreateIncomeUseCase createIncomeUseCase, IGetTransactionCatalogsUseCase getTransactionCatalogsUseCase)
        {
            _navigation = navigation;
            _createIncomeUseCase = createIncomeUseCase;
            _getTransactionCatalogsUseCase = getTransactionCatalogsUseCase;

            GetCatalogs();
        }

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

        [RelayCommand]
        private async Task SaveIncome()
        {
            IsBusy = true;
            try
            {
                var income = new Income
                {
                    Date = Date,
                    Amount = Amount,
                    Description = Description,
                    PaymentTypeId = SelectedPaymentType?.Id,
                    TagIds = Tags.Where(t => t.IsSelected).Select(t => t.Id).ToList()
                };

                var result = await _createIncomeUseCase.ExecuteAsync(income);
                if (result.IsSuccessful)
                {
                    App.Alert.ShowToast("Income saved successfully.");
                    await _navigation.PopAsync();
                }
            }
            catch
            {
                App.Alert.ShowAlert("Error", "Could not save data.");
            }
            finally
            {
                IsBusy = false;
            }
        }

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
            catch
            {
                App.Alert.ShowAlert("Error", "Could not load payment types and tags.");
            }
        }
    }
}
