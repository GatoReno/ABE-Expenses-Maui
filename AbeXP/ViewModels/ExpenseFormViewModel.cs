using AbeXP.Models;
using AbeXP.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AbeXP.UseCases.Interfaces;

namespace AbeXP.ViewModels
{
    public partial class ExpenseFormViewModel : ObservableObject
    {
        private readonly IGetTransactionCatalogsUseCase _getTransactionCatalogsUseCase;
        private readonly ICreateExpenseUseCase _createExpenseUseCase;
        private readonly INavigationService _navigation;

        public ExpenseFormViewModel(INavigationService navigation, ICreateExpenseUseCase createExpenseUseCase,IGetTransactionCatalogsUseCase getTransactionCatalogsUseCase)
        {
            _navigation = navigation;
            _createExpenseUseCase = createExpenseUseCase;
            _getTransactionCatalogsUseCase = getTransactionCatalogsUseCase;

            // Simulación de datos iniciales
            //PaymentMethods.Add(new PaymentMethod { Id = "1", Name = "Tarjeta" });
            //PaymentMethods.Add(new PaymentMethod { Id = "2", Name = "Efectivo" });
            //PaymentMethods.Add(new PaymentMethod { Id = "3", Name = "Transferencia" });
            //Tags.Add(new TagModel { Id = "1", Name = "Comida", ColorHex = "#FF5733" });
            //Tags.Add(new TagModel { Id = "2", Name = "Ropa", ColorHex = "#33FF57" });
            //Tags.Add(new TagModel { Id = "3", Name = "Transporte", ColorHex = "#3357FF" });

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
        private string selectedPaymentTypeId;


        public ObservableCollection<TagModel> Tags { get; set; }

        public ObservableCollection<PaymentMethod> PaymentMethods { get; set; }
        #endregion

        [RelayCommand]
        private async Task SaveExpense()
        {
            try
            {
                var expense = new Expense
                {
                    Date = Date,
                    Amount = Amount,
                    Description = Description,
                    PaymentTypeId = SelectedPaymentTypeId,
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
                App.Alert.ShowAlert("Error", "Could not save expenses data.");
            }
        }


        private async Task GetCatalogs()
        {
            try
            {
                var result = await _getTransactionCatalogsUseCase.ExecuteAsync();
                if (result.IsSuccessful)
                {
                    Tags = new ObservableCollection<TagModel>(result.Payload.Tags);
                    PaymentMethods = new ObservableCollection<PaymentMethod>(result.Payload.PaymentMethods);
                }
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not load payment types and tags.");
            }
        }
    }
}
