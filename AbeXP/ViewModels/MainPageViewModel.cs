using AbeXP.Common.Enum;
using AbeXP.Extensions;
using AbeXP.Models;
using AbeXP.Resources.Strings;
using AbeXP.UseCases.Interfaces;
using AbeXP.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AbeXP.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IGetTransactionsUseCase _getTransactionsUseCase;
        
        public MainPageViewModel(IGetTransactionsUseCase getExpendituresUseCase)
        {
            _getTransactionsUseCase = getExpendituresUseCase;
            LoadTransactionsAsync();
        }


        #region
        [ObservableProperty]
        public bool _isBusy;

        [ObservableProperty] 
        private List<string> filters = new() { AppResources.All, AppResources.Expense, AppResources.Loan};
        
        [ObservableProperty] 
        private string selectedFilter = AppResources.All;
        private string titleFilter = AppResources.FilterTransactions;


        private IReadOnlyCollection<TransactionItem> _allItems;
        public IReadOnlyCollection<TransactionItem> AllItems
        {
            get { return _allItems; }
            set
            {
                _allItems = value;
                ApplyFilters();
            }
        }

        [ObservableProperty] 
        private ObservableCollection<TransactionItem> _transactions = new();

        // date pickers
        [ObservableProperty]
        public DateTime _startDate = DateTime.Now.FirstDayOfCurrentMonth();
        [ObservableProperty]
        public DateTime _endDate = DateTime.Now.LastDayOfCurrentMonth();
        #endregion

        /// <summary>
        /// Trigger when the Selected filter for the type of transactions changes
        /// </summary>
        /// <param name="value"></param>
        partial void OnSelectedFilterChanged(string value)
        {
            ApplyFilters();
        }


        /// <summary>
        /// Load transactions
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task LoadTransactionsAsync()
        {
            IsBusy = true;
            try
            {
                var transactionsResult = await _getTransactionsUseCase.ExecuteAsync(new TransactionRequest()
                {
                    StartAt = StartDate,
                    EndAt = EndDate
                });

                AllItems =  new List<TransactionItem>(transactionsResult.Payload);
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

        /// <summary>
        /// Create new transaction
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task AddTransactionAsync()
        {
            string action = await App.Current.MainPage.DisplayActionSheet(
                AppResources.AddTransaction,
                AppResources.Cancel,
                null,
                AppResources.Expense,
                AppResources.Loan);

            if (action == AppResources.Expense)
                await Shell.Current.GoToAsync(nameof(ExpenseFormView));
            else if (action == AppResources.Loan)
                await Shell.Current.GoToAsync(nameof(LoanFormView));
        }

        /// <summary>
        /// Filter transactions based on the selected filter
        /// </summary>
        [RelayCommand]
        private void ApplyFilters()
        {
            Transactions.Clear();

            var filtered = _allItems.AsEnumerable();

            if (SelectedFilter == AppResources.Expense)
                filtered = filtered.Where(x => x.Type == TransactionType.Expense);
            else if (SelectedFilter == AppResources.Loan)
                filtered = filtered.Where(x => x.Type == TransactionType.Loan);

            Transactions = new ObservableCollection<TransactionItem>(filtered);
        }
    }
}
