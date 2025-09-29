using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AbeXP.Models;
using AbeXP.Views;
using AbeXP.UseCases.Interfaces;

namespace AbeXP.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IGetTransactionsUseCase _getTransactionsUseCase;
        private List<object> _allItems = new();

        public MainPageViewModel(IGetTransactionsUseCase getExpendituresUseCase)
        {
            _getTransactionsUseCase = getExpendituresUseCase;
            LoadTransactions();
        }



        [ObservableProperty]
        public bool _isBusy;

        [ObservableProperty] 
        private List<string> filters = new() { "Todos", "Expenses", "Loans" };
        
        [ObservableProperty] 
        private string selectedFilter = "Todos";
        
        [ObservableProperty] 
        private DateTime selectedDate = DateTime.Today;
        
        [ObservableProperty] 
        private ObservableCollection<TransactionItem> transactions = new();

        

        private async void LoadTransactions()
        {
            IsBusy = true;
            try
            {
                var expenses = await _getTransactionsUseCase.ExecuteAsync(new TransactionRequest());
                Transactions =  new ObservableCollection<TransactionItem>(expenses);
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not load transactions.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Botón ➕
        [RelayCommand]
        private async Task AddTransactionAsync()
        {
            string action = await App.Current.MainPage.DisplayActionSheet(
                "Agregar Transacción",
                "Cancelar",
                null,
                "Expense",
                "Loan");

            if (action == "Expense")
                await Shell.Current.GoToAsync(nameof(ExpenseFormView));
            else if (action == "Loan")
                await Shell.Current.GoToAsync(nameof(LoanFormView));
        }

        // Filtros
        [RelayCommand]
        private void ApplyFilters()
        {
            Transactions.Clear();

            var filtered = _allItems.AsEnumerable();

            if (SelectedFilter == "Expenses")
                filtered = filtered.Where(x => x is Expense);
            else if (SelectedFilter == "Loans")
                filtered = filtered.Where(x => x is Loan);

            // 🔹 Comparación directa (sin .Value porque SelectedDate es DateTime no nullable)
            if (SelectedDate != default)
            {
                filtered = filtered.Where(x =>
                    (x is Expense e && e.Date.Date == SelectedDate.Date) ||
                    (x is Loan l && l.DateGiven.Date == SelectedDate.Date));
            }

            var grouped = filtered
                .GroupBy(x =>
                    x is Expense e ? e.Date.Date :
                    x is Loan l ? l.DateGiven.Date : DateTime.MinValue)
                .OrderByDescending(g => g.Key);

            //foreach (var group in grouped)
                //Transactions.Add(new TransactionGroup(group.Key, group.ToList()));
        }
    }


    public class TransactionGroup : ObservableCollection<object>
    {
        public DateTime Date { get; private set; }
        public string DateString => Date.ToString("dd/MM/yyyy");

        public TransactionGroup(DateTime date, IEnumerable<object> items) : base(items)
        {
            Date = date;
        }
    }
}
