using AbeXP.Common.Enum;
using AbeXP.Extensions;
using AbeXP.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace AbeXP.ViewModels
{
    public partial class FinantialChartsViewModel : ObservableObject
    {

        public FinantialChartsViewModel()
        {
            FillExpenses();
            CreateExpensesLineChart();
            CreatePaymentTypesPieChart();
            CreateTagsBarChart();


        }

        #region PROPERTIES

        // data
        [ObservableProperty]
        public ObservableCollection<Expense> _expenses;

        // charts
        [ObservableProperty]
        public Chart _expensesLineChart;
        [ObservableProperty]
        public Chart _paymentsTypeDonutChart;
        [ObservableProperty]
        public Chart _tagsBarChart;
        [ObservableProperty]
        public TimePeriod _period = TimePeriod.ThreeDays;

        // dates configuration
        [ObservableProperty]
        public DateTime _startDate = DateTime.Now.FirstDayOfCurrentMonth();
        [ObservableProperty]
        public DateTime _minStartDateAllowed = DateTime.MinValue;
        [ObservableProperty]
        public DateTime _maxEndDateAllowed = DateTime.Now.LastDayOfCurrentMonth();
        [ObservableProperty]
        public DateTime _endDate = DateTime.Now.LastDayOfCurrentMonth();
        #endregion

        /// <summary>
        /// Triggered when the Period property changes, recreating the expenses line chart to reflect the new grouping.
        /// </summary>
        /// <param name="period"></param>
        partial void OnPeriodChanged(TimePeriod period)
        {
            CreateExpensesLineChart();
        }

        /// <summary>
        /// Creates a line chart representing expenses over time, grouped by the selected period (ThreeDays, Week, Month).
        /// </summary>
        private void CreateExpensesLineChart()
        {
            var groupedDates = Expenses
               .GroupBy(e => e.Date.GetPeriodStart(Period))
               .OrderBy(g => g.Key)
               .Select(g => new { Date = g.Key, Total = g.Sum(e => e.Amount) });

            var dateEntries = groupedDates.Select(g => new ChartEntry((float)g.Total)
            {
                Label = Period switch
                {
                    TimePeriod.ThreeDays => g.Date.ToString("MM-dd"),
                    TimePeriod.Week => $"Week of {g.Date:MM-dd}",
                    TimePeriod.Month => g.Date.ToString("MMM yyyy"),
                    _ => g.Date.ToString("MM-dd")
                },
                ValueLabel = g.Total.ToString("C"),
                Color = SKColors.DeepSkyBlue
            }).ToArray();

            ExpensesLineChart = new LineChart
            {
                Entries = dateEntries,
                LineMode = LineMode.Straight,
                LineSize = 2,
                PointMode = PointMode.Circle,
                PointSize = 5,
                BackgroundColor = SKColors.Transparent,
                LabelOrientation = Orientation.Vertical
            };
        }

        /// <summary>
        /// Creates a donut chart representing the distribution of expenses by payment types.
        /// </summary>
        private void CreatePaymentTypesPieChart()
        {
            var grouped = Expenses
            .GroupBy(e => e.PaymentTypeId)
            .Select(g => new
            {
                g.Key,
                Total = g.Sum(e => e.Amount)
            });

            var entries = grouped.Select(g => new ChartEntry((float)g.Total)
            {
                Label = g.Key,
                ValueLabel = g.Total.ToString("C"),
                Color = SKColor.Parse($"#{new Random().Next(0x1000000):X6}")
            }).ToArray();

            PaymentsTypeDonutChart = new DonutChart
            {
                Entries = entries,
                HoleRadius = 0.6f,
                BackgroundColor = SKColors.Transparent
            };
        }
       
        /// <summary>
        /// Creates a bar chart representing the total expenses associated with each tag.
        /// </summary>
        private void CreateTagsBarChart()
        {
            var grouped = Expenses
                .SelectMany(e => e.TagIds.Select(tagId => new { TagId = tagId, e.Amount }))
                .GroupBy(x => x.TagId)
                .Select(g => new
                {
                    g.Key,
                    Total = g.Sum(x => x.Amount)
                });

            var entries = grouped.Select(g => new ChartEntry((float)g.Total)
            {
                Label = g.Key,
                ValueLabel = g.Total.ToString("C"),
                Color = SKColor.Parse($"#{new Random().Next(0x1000000):X6}"),
            }).ToArray();

            TagsBarChart = new BarChart
            {
                Entries = entries,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                BackgroundColor = SKColors.Transparent,
                BarAreaAlpha = 0,
                MaxValue = (float)(entries.Max(e => e.Value) * 1.1f) // small padding above
            };
        }

        private void FillExpenses()
        {
            Expenses = new ObservableCollection<Expense>
            {
                new Expense { Date = DateTime.Now.AddDays(-1), Amount = 45.20m, Description = "Groceries at Lidl", PaymentTypeId = "DebitCard", TagIds = new List<string>{ "Food" } },
                new Expense { Date = DateTime.Now.AddDays(-2), Amount = 12.50m, Description = "Bus ticket", PaymentTypeId = "Cash", TagIds = new List<string>{ "Transport" } },
                new Expense { Date = DateTime.Now.AddDays(-3), Amount = 80.00m, Description = "Dinner out", PaymentTypeId = "CreditCard", TagIds = new List<string>{ "Food", "Entertainment" } },
                new Expense { Date = DateTime.Now.AddDays(-4), Amount = 150.00m, Description = "Electricity bill", PaymentTypeId = "BankTransfer", TagIds = new List<string>{ "Bills" } },
                new Expense { Date = DateTime.Now.AddDays(-5), Amount = 30.00m, Description = "Movie tickets", PaymentTypeId = "MobilePay", TagIds = new List<string>{ "Entertainment" } },
                new Expense { Date = DateTime.Now.AddDays(-6), Amount = 22.90m, Description = "Lunch at work", PaymentTypeId = "DebitCard", TagIds = new List<string>{ "Food" } },
                new Expense { Date = DateTime.Now.AddDays(-7), Amount = 75.00m, Description = "Shoes", PaymentTypeId = "CreditCard", TagIds = new List<string>{ "Shopping" } },
                new Expense { Date = DateTime.Now.AddDays(-8), Amount = 60.00m, Description = "Pharmacy", PaymentTypeId = "Cash", TagIds = new List<string>{ "Health" } },
                new Expense { Date = DateTime.Now.AddDays(-9), Amount = 95.00m, Description = "Fuel for car", PaymentTypeId = "DebitCard", TagIds = new List<string>{ "Transport" } },
                new Expense { Date = DateTime.Now.AddDays(-10), Amount = 20.00m, Description = "Gym pass", PaymentTypeId = "BankTransfer", TagIds = new List<string>{ "Health" } },
                new Expense { Date = DateTime.Now.AddDays(-11), Amount = 35.00m, Description = "Clothes shopping", PaymentTypeId = "CreditCard", TagIds = new List<string>{ "Shopping" } },
                new Expense { Date = DateTime.Now.AddDays(-12), Amount = 200.00m, Description = "Weekend trip hotel", PaymentTypeId = "CreditCard", TagIds = new List<string>{ "Entertainment", "Travel" } },
                new Expense { Date = DateTime.Now.AddDays(-13), Amount = 15.00m, Description = "Coffee with friends", PaymentTypeId = "Cash", TagIds = new List<string>{ "Food" } },
                new Expense { Date = DateTime.Now.AddDays(-14), Amount = 110.00m, Description = "Monthly internet", PaymentTypeId = "BankTransfer", TagIds = new List<string>{ "Bills" } },
                new Expense { Date = DateTime.Now.AddDays(-15), Amount = 50.00m, Description = "Concert ticket", PaymentTypeId = "MobilePay", TagIds = new List<string>{ "Entertainment" } },
                new Expense { Date = DateTime.Now.AddDays(-16), Amount = 18.50m, Description = "Groceries – fruits", PaymentTypeId = "DebitCard", TagIds = new List<string>{ "Food" } },
                new Expense { Date = DateTime.Now.AddDays(-17), Amount = 250.00m, Description = "New phone", PaymentTypeId = "CreditCard", TagIds = new List<string>{ "Shopping", "Electronics" } },
                new Expense { Date = DateTime.Now.AddDays(-18), Amount = 40.00m, Description = "Taxi ride", PaymentTypeId = "Cash", TagIds = new List<string>{ "Transport" } },
                new Expense { Date = DateTime.Now.AddDays(-19), Amount = 90.00m, Description = "Doctor visit", PaymentTypeId = "BankTransfer", TagIds = new List<string>{ "Health" } },
                new Expense { Date = DateTime.Now.AddDays(-20), Amount = 10.00m, Description = "Ice cream", PaymentTypeId = "Cash", TagIds = new List<string>{ "Food" } },
            };
        }


    }
}
