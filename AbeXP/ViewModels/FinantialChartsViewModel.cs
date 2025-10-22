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
        public Chart _incomesLineChart;
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
        public decimal? _totalIncomeAmount;
        [ObservableProperty]
        public int _transactionsCount;


        #endregion

        /// <summary>
        /// Triggered when the Period property changes, recreating the expenses line chart to reflect the new grouping.
        /// </summary>
        /// <param name="period"></param>
        partial void OnPeriodChanged(TimePeriod period)
        {
            IsBusy = true;
            try
            {
                CreateExpensesLineChart();
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
            SKColor labelColor = IsDarkMode ? SKColors.White : SKColors.Black;

            var grouped = Transactions
                .GroupBy(t => new { Period = t.Date.GetPeriodStart(Period), t.Type })
                .Select(g => new { Date = g.Key.Period, Type = g.Key.Type, Total = g.Sum(t => t.Amount) })
                .ToList();


            var expenses = grouped
                .Where(g => g.Type == TransactionType.Expense)
                .OrderBy(g => g.Date)
                .Select(g => new ChartEntry((float)g.Total)
                {
                    Label = Period.ToLabel(g.Date),
                    ValueLabel = g.Total.ToString("C"),
                    Color = SKColor.Parse("#E74C3C"), // red
                    ValueLabelColor = labelColor
                })
                .ToList();

            var incomes = grouped
                .Where(g => g.Type == TransactionType.Income)
                .OrderBy(g => g.Date)
                .Select(g => new ChartEntry((float)g.Total)
                {
                    Label = Period.ToLabel(g.Date),
                    ValueLabel = g.Total.ToString("C"),
                    Color = SKColor.Parse("#27AE60"), // green
                    ValueLabelColor = labelColor
                })
                .ToList();



            ExpensesLineChart = new LineChart
            {
                AnimationDuration = TimeSpan.Zero,
                LineAreaAlpha = 0,
                Entries = expenses,
                LineMode = LineMode.Straight,
                LineSize = 4,
                PointMode = PointMode.Circle,
                PointSize = 5,
                BackgroundColor = SKColors.Transparent,
                LabelTextSize = ChartLabelFontSize,
                LabelColor = labelColor
            };

            IncomesLineChart = new LineChart
            {
                LineAreaAlpha = 0,
                Entries = incomes,
                LineMode = LineMode.Straight,
                LineSize = 4,
                PointMode = PointMode.Circle,
                PointSize = 5,
                AnimationDuration = TimeSpan.Zero,
                BackgroundColor = SKColors.Transparent,
                LabelTextSize = ChartLabelFontSize,
                LabelColor = labelColor
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

                var transactions = transactionsResult.Value.ToList();
                TransactionsCount = transactions.Any() ? transactions.Count : 0;

                if (!transactions.Any(t => t.Type == TransactionType.Expense))
                {
                    transactions.Add(new TransactionItem
                    {
                        Amount = 0,
                        Date = DateTime.Now,
                        Description = "No expenses found",
                        PaymentMethod = "N/A",
                        Type = TransactionType.Expense,
                        Tags = new List<string> { "N/A" }
                    });
                }

                if (!transactions.Any(t => t.Type == TransactionType.Income))
                {
                    transactions.Add(new TransactionItem
                    {
                        Amount = 0,
                        Date = DateTime.Now,
                        Description = "No inconmes found",
                        PaymentMethod = "N/A",
                        Type = TransactionType.Income,
                        Tags = new List<string> { "N/A" }
                    });

                }


                var minDate = transactions.MinBy(e => e.Date).Date;
                var maxDate = transactions.MaxBy(e => e.Date).Date;

                TotalExpensesAmount = transactions?.Where(t => t.Type == TransactionType.Expense).Sum(e => e.Amount);
                TotalIncomeAmount = transactions?.Where(t => t.Type == TransactionType.Income).Sum(e => e.Amount);
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
