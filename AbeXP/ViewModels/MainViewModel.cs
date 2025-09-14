using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AbeXP.Models;

namespace AbeXP.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        public ObservableCollection<TransactionGroup> Transactions { get; set; } = new();

        [ObservableProperty] private string selectedFilter = "Todos"; // "Todos", "Expense", "Loan"
        [ObservableProperty] private DateTime? selectedDate;

        public MainPageViewModel()
        {
            LoadMockData();
            ApplyFilters();
        }

        private ObservableCollection<object> _allItems = new();

        private void LoadMockData()
        {
            // Mock Expenses
            var expense1 = new Expense { Amount = 100, Description = "Comida", Date = DateTime.Today, PaymentTypeId = "cash" };
            var expense2 = new Expense { Amount = 50, Description = "Taxi", Date = DateTime.Today.AddDays(-1), PaymentTypeId = "cash" };

            // Mock Loans
            var loan1 = new Loan { PersonName = "Juan Pérez", Amount = 500, DateGiven = DateTime.Today };
            var loan2 = new Loan { PersonName = "Ana López", Amount = 1200, DateGiven = DateTime.Today.AddDays(-1), IsPaid = true };

            _allItems = new ObservableCollection<object> { expense1, expense2, loan1, loan2 };
        }

        [RelayCommand]
        private void ApplyFilters()
        {
            Transactions.Clear();

            var filtered = _allItems.AsEnumerable();

            if (SelectedFilter == "Expense")
                filtered = filtered.Where(x => x is Expense);
            else if (SelectedFilter == "Loan")
                filtered = filtered.Where(x => x is Loan);

            if (SelectedDate != null)
            {
                filtered = filtered.Where(x =>
                    (x is Expense e && e.Date.Date == SelectedDate.Value.Date) ||
                    (x is Loan l && l.DateGiven.Date == SelectedDate.Value.Date));
            }

            var grouped = filtered
                .GroupBy(x =>
                    x is Expense e ? e.Date.Date :
                    x is Loan l ? l.DateGiven.Date : DateTime.MinValue)
                .OrderByDescending(g => g.Key);

            foreach (var group in grouped)
            {
                var transactionGroup = new TransactionGroup(group.Key, group.ToList());
                Transactions.Add(transactionGroup);
            }
        }
    }

    public class TransactionGroup : ObservableCollection<object>
    {
        public DateTime Date { get; private set; }

        public string DateString => Date.ToString("dd/MM/yyyy");

        public TransactionGroup(DateTime date, System.Collections.Generic.IEnumerable<object> items) : base(items)
        {
            Date = date;
        }
    }
}

