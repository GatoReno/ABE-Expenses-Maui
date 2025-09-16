using AbeXP.Common.Enum;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace AbeXP.ViewModels
{
    public partial class FinantialChartsViewModel : ObservableObject
    {
        private readonly IExpenseRepository _expenseRepository;

        public FinantialChartsViewModel(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;


            GetExpenses().ContinueWith((t) =>
            {
                CreateExpensesLineChart();
                CreatePaymentTypesPieChart();
                CreateTagsBarChart();
            });

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
                    TimePeriod.ThreeDays => g.Date.ToString("MMM-dd"),
                    TimePeriod.Week => $"Week {g.Date:MMM-dd}",
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

        private async Task GetExpenses()
        {
            try
            {
                var expenses = await _expenseRepository.GetAllAsync();
                Expenses = new ObservableCollection<Expense>(expenses);
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not load expenses data.");
            }
        }
    }
}
