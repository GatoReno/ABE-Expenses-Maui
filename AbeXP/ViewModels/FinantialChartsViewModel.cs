using AbeXP.Common.Enum;
using AbeXP.Extensions;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.Resources.Strings;
using AbeXP.UseCases.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace AbeXP.ViewModels
{
    public partial class FinantialChartsViewModel : ObservableObject
    {
        private readonly IGetTransactionsUseCase _getTransactionsUseCase;

        public FinantialChartsViewModel(IGetTransactionsUseCase getTransactionsUseCase)
        {
            _getTransactionsUseCase = getTransactionsUseCase;

            GetTransactionsAsync();
        }

        #region PROPERTIES

        // data
        private IReadOnlyList<TransactionItem> _transactions;
        public IReadOnlyList<TransactionItem> Transactions
        {
            get { return _transactions; }
            set
            {
                _transactions = value;
                OnTransactionsChanged();
            }
        }
        public bool IsDarkMode => Application.Current.RequestedTheme == AppTheme.Dark;
        public float ChartLabelFontSize => 40f;

        [ObservableProperty]
        public bool _expensesMoreThanOneMonth;

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
        public DateTime _endDate = DateTime.Now.LastDayOfCurrentMonth();
        [ObservableProperty]
        public DateTime _maxEndDateAllowed = DateTime.Now.LastDayOfCurrentMonth();

        // total
        [ObservableProperty]
        public decimal? _totalExpensesAmount;
        [ObservableProperty]
        public int _totalExpensesCount;

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
        private void OnTransactionsChanged()
        {
            IsBusy = true;
            try
            {
                CreateExpensesLineChart();
                CreatePaymentTypesPieChart();
                CreateTagsBarChart();


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
            var groupedDates = Transactions
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
                LabelTextSize = ChartLabelFontSize,
            };
        }

        /// <summary>
        /// Creates a donut chart representing the distribution of expenses by payment types.
        /// </summary>
        private void CreatePaymentTypesPieChart()
        {
            var grouped = Transactions
                .GroupBy(e => e.PaymentMethod)
                .Select(g => new
                {
                    g.Key,
                    Count = g.Count(),
                    Total = g.Sum(e => e.Amount)
                });

            var entries = grouped.Select(g => new ChartEntry((float)g.Total)
            {
                Label = $"{g.Key} ({g.Count})",
                ValueLabel = g.Total.ToString("C"),
                ValueLabelColor = IsDarkMode ? SKColors.White : SKColors.Black,
                Color = SKColor.Parse($"#{new Random().Next(0x1000000):X6}")
            }).ToArray();

            PaymentsTypeDonutChart = new DonutChart
            {
                Entries = entries,
                HoleRadius = 0.6f,
                BackgroundColor = SKColors.Transparent,
                LabelColor = IsDarkMode ? SKColors.White : SKColors.Black,
                LabelTextSize = ChartLabelFontSize,
            };
        }

        /// <summary>
        /// Creates a bar chart representing the total expenses associated with each tag.
        /// </summary>
        private void CreateTagsBarChart()
        {
            // divide amount on all the transaction tags, since a single transaction may contain more than one tag
            var grouped = Transactions
                .SelectMany(e => e.Tags?.Select(tag => new { Tag = tag, Amount = e.Amount / e.Tags.Count }) ?? [new { Tag = AppResources.NoTag, e.Amount }])
                .GroupBy(x => x.Tag)
                .Select(g => new
                {
                    g.Key,
                    Count = g.Count(),
                    Total = g.Sum(x => x.Amount)
                });

            var entries = grouped.Select(g => new ChartEntry((float)g.Total)
            {
                Label = $"{g.Key} ({g.Count})",
                ValueLabelColor = IsDarkMode ? SKColors.White : SKColors.Black,
                ValueLabel = g.Total.ToString("C"),
                Color = SKColor.Parse($"#{new Random().Next(0x1000000):X6}"),
            });

            //if so, the barchart takes all width screen, reduce by adding dummy entries
            if (entries.Count() == 1)
            {
                entries = entries.Prepend(new ChartEntry(0)
                {
                    Label = "",
                    ValueLabel = "",
                    Color = SKColor.Parse("#00FFFFFF")
                });

                entries = entries.Append(new ChartEntry(0)
                {
                    Label = "",
                    ValueLabel = "",
                    Color = SKColor.Parse("#00FFFFFF")
                });
            }

            TagsBarChart = new BarChart
            {
                Entries = entries,
                BackgroundColor = SKColors.Transparent,
                BarAreaAlpha = 0,
                MaxValue = entries.Any() ? (float)(entries.Max(e => e.Value) * 1.1f) : 0f, // small padding above
                LabelColor = IsDarkMode ? SKColors.White : SKColors.Black,
                LabelTextSize = ChartLabelFontSize,
            };
        }


        /// <summary>
        /// Get all expenses from the repository and populate the Expenses collection.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GetTransactionsAsync()
        {
            IsBusy = true;
            try
            {
                var transactionsResult = await _getTransactionsUseCase.ExecuteAsync(new TransactionRequest
                {
                    StartAt = StartDate,
                    EndAt = EndDate,
                    MapTags = true
                });

                if (transactionsResult.IsFailed)
                {
                    App.Alert.ShowAlert("Error", "Could not load data.");
                }

                var transactions = transactionsResult.Payload.ToList();

                if (!transactions.Any())
                {
                    transactions.Add(new TransactionItem
                    {
                        Amount = 0,
                        Date = DateTime.Now,
                        Description = "No expenses found",
                        PaymentMethod = "N/A",
                        Tags = new List<string> { "N/A" }
                    });

                    TotalExpensesCount = 0;
                }
                else
                {
                    TotalExpensesCount = transactions.Count;
                }


                var minDate = transactions.MinBy(e => e.Date).Date;
                var maxDate = transactions.MaxBy(e => e.Date).Date;

                TotalExpensesAmount = transactions?.Sum(e => e.Amount);
                ExpensesMoreThanOneMonth = minDate.IsMoreThanOneMonthApart(maxDate);
                Transactions = transactions.AsReadOnly();
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not load data.");
            }
            finally
            {
                IsBusy = false;
            }
        }


    }
}
