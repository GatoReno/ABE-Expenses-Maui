using AbeXP.Common.DTO;
using AbeXP.Common.Enum;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

            GetExpenses();
        }

        #region PROPERTIES

        // data
        [ObservableProperty]
        public ObservableCollection<Expense> _expenses;
        public bool IsDarkMode => Application.Current.RequestedTheme == AppTheme.Dark;

        [ObservableProperty]
        public bool _isBusy;

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

        // total
        [ObservableProperty]
        public decimal? _totalExpenses;


        // titles
        [ObservableProperty]
        public string _totalExpensesTitle = "Gasto total";
        [ObservableProperty]
        public string _expensesChartsTitle = "Gráfico de gastos";
        [ObservableProperty]
        public string _timePeriodTitle = "Intervalo de tiempo";
        [ObservableProperty]
        public string _paymentTypeChartTitle = "Por tipo de pago";
        [ObservableProperty]
        public string _tagsTypeChartTitle = "Por etiquetas";


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
        /// Triggered when the Expenses property changes, refresh all the charts with the new data
        /// </summary>
        /// <param name="value"></param>
        partial void OnExpensesChanged(ObservableCollection<Expense> value)
        {
            IsBusy = true;
            try
            {
                CreateExpensesLineChart();
                CreatePaymentTypesPieChart();
                CreateTagsBarChart();

                TotalExpenses = Expenses?.Sum(e => e.Amount);
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not load charts.");
            }
            finally
            {
                IsBusy = false;
            }
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
                ValueLabelColor = IsDarkMode ? SKColors.White : SKColors.Black,
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
                LabelOrientation = Orientation.Vertical,
                LabelColor = IsDarkMode ? SKColors.White : SKColors.Black,
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
                ValueLabelColor = IsDarkMode ? SKColors.White : SKColors.Black,
                Color = SKColor.Parse($"#{new Random().Next(0x1000000):X6}")
            }).ToArray();

            PaymentsTypeDonutChart = new DonutChart
            {
                Entries = entries,
                HoleRadius = 0.6f,
                BackgroundColor = SKColors.Transparent,
                LabelColor = IsDarkMode ? SKColors.White : SKColors.Black
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
                ValueLabelColor = IsDarkMode ? SKColors.White : SKColors.Black,
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
                MaxValue = entries.Any() ? (float)(entries.Max(e => e.Value) * 1.1f) : 0f, // small padding above
                LabelColor = IsDarkMode ? SKColors.White : SKColors.Black
            };
        }


        /// <summary>
        /// Get all expenses from the repository and populate the Expenses collection.
        /// </summary>
        /// <returns></returns>
        private async Task GetExpenses()
        {
            IsBusy = true;
            try
            {
                var expenses = await _expenseRepository.GetAllAsync();
                Expenses = new ObservableCollection<Expense>(expenses);
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not load expenses data.");
            }
            finally
            { 
                IsBusy = false; 
            }
        }


        /// <summary>
        /// Handles the search command to filter expenses based on the selected date range.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task Search()
        {
            await GetExpenses();
        }

    }
}
