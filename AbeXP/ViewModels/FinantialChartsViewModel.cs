using AbeXP.Common.Enum;
using AbeXP.Extensions;
using AbeXP.Models;
using AbeXP.Resources.Strings;
using AbeXP.UseCases.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Measure;
using Microsoft.Maui.Controls;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace AbeXP.ViewModels
{
    public partial class FinantialChartsViewModel : ObservableObject
    {
        private readonly IGetTransactionsUseCase _getTransactionsUseCase;

        private static readonly SKColor ExpenseColor = SKColor.Parse("#E74C3C");
        private static readonly SKColor IncomeColor = SKColor.Parse("#27AE60");
        private static readonly SKColor TagsColor = SKColor.Parse("#3498DB");
        private static readonly SKColor[] Palette =
        [
            SKColor.Parse("#FF6B6B"),
            SKColor.Parse("#6BCB77"),
            SKColor.Parse("#4D96FF"),
            SKColor.Parse("#FFBC42"),
            SKColor.Parse("#9D4EDD"),
            SKColor.Parse("#FF8FAB"),
            SKColor.Parse("#4ECDC4")
        ];

        public FinantialChartsViewModel(IGetTransactionsUseCase getTransactionsUseCase)
        {
            _getTransactionsUseCase = getTransactionsUseCase;

            TransactionLineSeries = new ObservableCollection<ISeries>();
            TransactionLineXAxes = Array.Empty<Axis>();
            TransactionLineYAxes = Array.Empty<Axis>();

            PaymentMethodSeries = new ObservableCollection<ISeries>();
            TagsSeries = new ObservableCollection<ISeries>();
            TagsXAxes = Array.Empty<Axis>();
            TagsYAxes = Array.Empty<Axis>();

            GetTransactionsAsync();

            Application.Current.RequestedThemeChanged += (s, a) =>
            {
                OnTransactionsChanged();
            };
        }

        #region PROPERTIES

        // data
        private IReadOnlyList<TransactionItem> _transactions = Array.Empty<TransactionItem>();
        public IReadOnlyList<TransactionItem> Transactions
        {
            get => _transactions;
            set
            {
                _transactions = value;
                OnTransactionsChanged();
            }
        }

        public bool IsDarkMode => Application.Current.RequestedTheme == AppTheme.Dark;

        [ObservableProperty]
        private bool _expensesMoreThanOneMonth;

        [ObservableProperty]
        private bool _isBusy;

        // charts
        [ObservableProperty]
        private ObservableCollection<ISeries> _transactionLineSeries;

        [ObservableProperty]
        private Axis[] _transactionLineXAxes;

        [ObservableProperty]
        private Axis[] _transactionLineYAxes;

        [ObservableProperty]
        private ObservableCollection<ISeries> _paymentMethodSeries;

        [ObservableProperty]
        private ObservableCollection<ISeries> _tagsSeries;

        [ObservableProperty]
        private Axis[] _tagsXAxes;

        [ObservableProperty]
        private Axis[] _tagsYAxes;

        [ObservableProperty]
        private TimePeriod _period = TimePeriod.ThreeDays;

        private SKColor GetTextColor() => IsDarkMode ? SKColors.White : SKColors.Black;

        private SKColor GetSeparatorColor() => IsDarkMode ? SKColor.Parse("#2E2E2E") : SKColor.Parse("#DDDDDD");

        private SKColor GetPaletteColor(int index) => Palette[index % Palette.Length];

        private sealed record PaymentMethodSlice(string PaymentMethod, decimal Total, int Count);

        private sealed record TagChartEntry(string Tag, decimal Total, int Count);

        // dates configuration
        [ObservableProperty]
        private DateTime _startDate = DateTime.Now.FirstDayOfCurrentMonth();

        [ObservableProperty]
        private DateTime _minStartDateAllowed = DateTime.MinValue;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now.LastDayOfCurrentMonth();

        [ObservableProperty]
        private DateTime _maxEndDateAllowed = DateTime.Now.LastDayOfCurrentMonth();

        // total
        [ObservableProperty]
        private decimal? _totalExpensesAmount;

        [ObservableProperty]
        private decimal? _totalIncomeAmount;

        [ObservableProperty]
        private int _transactionsCount;

        #endregion

        /// <summary>
        /// Triggered when the Period property changes; refresh the line chart grouping.
        /// </summary>
        /// <param name="period">Selected period grouping.</param>
        partial void OnPeriodChanged(TimePeriod period)
        {
            try
            {
                IsBusy = true;
                CreateTransactionLineChart();
            }
            catch
            {
                App.Alert.ShowAlert("Error", "Could not load charts.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Triggered when the Transactions collection changes; rebuild all charts.
        /// </summary>
        private void OnTransactionsChanged()
        {
            try
            {
                IsBusy = true;
                CreateTransactionLineChart();
                CreatePaymentMethodsPieChart();
                CreateTagsRowChart();
            }
            catch
            {
                App.Alert.ShowAlert("Error", "Could not load charts.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Creates a line chart that plots expenses and incomes over the selected period aggregation.
        /// </summary>
        private void CreateTransactionLineChart()
        {
            if (Transactions is null || Transactions.Count == 0)
            {
                TransactionLineSeries = new ObservableCollection<ISeries>();
                TransactionLineXAxes = Array.Empty<Axis>();
                TransactionLineYAxes = Array.Empty<Axis>();
                return;
            }

            var grouped = Transactions
                .GroupBy(t => new { Period = t.Date.GetPeriodStart(Period), t.Type })
                .Select(g => new
                {
                    g.Key.Period,
                    g.Key.Type,
                    Total = g.Sum(t => t.Amount)
                })
                .ToList();

            var periods = grouped
                .Select(g => g.Period)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (periods.Count == 0)
            {
                TransactionLineSeries = new ObservableCollection<ISeries>();
                TransactionLineXAxes = Array.Empty<Axis>();
                TransactionLineYAxes = Array.Empty<Axis>();
                return;
            }

            var expenses = periods
                .Select(date => (double)(grouped.FirstOrDefault(g => g.Period == date && g.Type == TransactionType.Expense)?.Total ?? 0m))
                .ToList();

            var incomes = periods
                .Select(date => (double)(grouped.FirstOrDefault(g => g.Period == date && g.Type == TransactionType.Income)?.Total ?? 0m))
                .ToList();

            var labelColor = GetTextColor();

            TransactionLineSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Name = AppResources.Expense,
                    Values = expenses,
                    Stroke = new SolidColorPaint(ExpenseColor) { StrokeThickness = 4 },
                    Fill = null,
                    GeometryStroke = new SolidColorPaint(ExpenseColor) { StrokeThickness = 4 },
                    GeometryFill = new SolidColorPaint(ExpenseColor),
                    GeometrySize = 10
                },
                new LineSeries<double>
                {
                    Name = AppResources.Income,
                    Values = incomes,
                    Stroke = new SolidColorPaint(IncomeColor) { StrokeThickness = 4 },
                    Fill = null,
                    GeometryStroke = new SolidColorPaint(IncomeColor) { StrokeThickness = 4 },
                    GeometryFill = new SolidColorPaint(IncomeColor),
                    GeometrySize = 10
                }
            };

            TransactionLineXAxes = new[]
            {
                new Axis
                {
                    Labels = periods.Select(p => Period.ToLabel(p)).ToArray(),
                    LabelsRotation = 70,
                    LabelsPaint = new SolidColorPaint(labelColor),
                    TextSize = 14,
                    Position = AxisPosition.End
                }
            };

            TransactionLineYAxes = new[]
            {
                new Axis
                {
                    MinLimit = 0,
                    Labeler = value => value.ToString("C0"),
                    LabelsPaint = new SolidColorPaint(labelColor),
                    TextSize = 14,
                    SeparatorsPaint = new SolidColorPaint(GetSeparatorColor()) { StrokeThickness = 1 }
                }
            };
        }

        /// <summary>
        /// Builds the pie chart aggregating transaction amounts by payment method.
        /// </summary>
        private void CreatePaymentMethodsPieChart()
        {
            if (Transactions is null || Transactions.Count == 0)
            {
                PaymentMethodSeries = new ObservableCollection<ISeries>();
                return;
            }

            var grouped = Transactions
                .GroupBy(t => string.IsNullOrWhiteSpace(t.PaymentMethod) ? "N/A" : t.PaymentMethod)
                .Select(g => new PaymentMethodSlice(g.Key, g.Sum(t => t.Amount), g.Count()))
                .OrderByDescending(slice => slice.Total)
                .ToList();

            if (grouped.Count == 0)
            {
                PaymentMethodSeries = new ObservableCollection<ISeries>();
                return;
            }

            var labelColor = GetTextColor();

            var pieSeries = new ObservableCollection<ISeries>();

            for (int i = 0; i < grouped.Count; i++)
            {
                var slice = grouped[i];
                pieSeries.Add(new PieSeries<ObservableValue>
                {
                    Name = slice.PaymentMethod,
                    Values = new ObservableCollection<ObservableValue> { new ObservableValue((double)slice.Total) },
                    Fill = new SolidColorPaint(GetPaletteColor(i)),
                    Stroke = new SolidColorPaint(SKColors.Transparent),
                    Pushout = 2
                });
            }

            PaymentMethodSeries = pieSeries;
        }

        /// <summary>
        /// Builds a row chart summarizing spend and transaction count by tag.
        /// </summary>
        private void CreateTagsRowChart()
        {
            if (Transactions is null || Transactions.Count == 0)
            {
                TagsSeries = new ObservableCollection<ISeries>();
                TagsXAxes = Array.Empty<Axis>();
                TagsYAxes = Array.Empty<Axis>();
                return;
            }

            var grouped = Transactions
                .SelectMany(t => t.Tags?.Select(tag => new { Tag = tag, Amount = t.Tags.Count > 0 ? t.Amount / t.Tags.Count : t.Amount }) ??
                                   new[] { new { Tag = AppResources.NoTag, Amount = t.Amount } })
                .GroupBy(x => x.Tag)
                .Select(g => new TagChartEntry(g.Key, g.Sum(x => x.Amount), g.Count()))
                .OrderByDescending(entry => entry.Total)
                .ToList();

            if (grouped.Count == 0)
            {
                TagsSeries = new ObservableCollection<ISeries>();
                TagsXAxes = Array.Empty<Axis>();
                TagsYAxes = Array.Empty<Axis>();
                return;
            }

            var labelColor = GetTextColor();

            var rowSeries = new RowSeries<ObservableValue>
            {
                Values = new ObservableCollection<ObservableValue>(grouped.Select(entry => new ObservableValue((double)entry.Total))),
                Fill = new SolidColorPaint(TagsColor),
                Stroke = new SolidColorPaint(SKColors.Transparent),
                XToolTipLabelFormatter = (point) =>
                {
                    var index = (int)Math.Clamp(Math.Round(point.Coordinate.SecondaryValue), 0, grouped.Count - 1);
                    var data = grouped[index];
                    return $"{data.Tag}{Environment.NewLine}{data.Total:C}";
                }
            };

            TagsSeries = new ObservableCollection<ISeries> { rowSeries };

            TagsXAxes = new[]
            {
                new Axis
                {
                    MinLimit = 0,
                    Labeler = value => value.ToString("C0"),
                    LabelsPaint = new SolidColorPaint(labelColor),
                    TextSize = 14,
                    SeparatorsPaint = new SolidColorPaint(GetSeparatorColor()) { StrokeThickness = 1 },
                    LabelsRotation = 90,
                    Position = AxisPosition.End
                }
            };

            TagsYAxes = new[]
            {
                new Axis
                {
                    Labels = grouped.Select(entry => entry.Tag).ToArray(),
                    LabelsPaint = new SolidColorPaint(labelColor),
                    TextSize = 14
                }
            };
        }

        /// <summary>
        /// Retrieves transactions and updates the dashboard.
        /// </summary>
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
                        Description = "No incomes found",
                        PaymentMethod = "N/A",
                        Type = TransactionType.Income,
                        Tags = new List<string> { "N/A" }
                    });
                }

                var minDate = transactions.MinBy(e => e.Date).Date;
                var maxDate = transactions.MaxBy(e => e.Date).Date;

                TotalExpensesAmount = transactions.Where(t => t.Type == TransactionType.Expense).Sum(e => e.Amount);
                TotalIncomeAmount = transactions.Where(t => t.Type == TransactionType.Income).Sum(e => e.Amount);
                ExpensesMoreThanOneMonth = minDate.IsMoreThanOneMonthApart(maxDate);
                Transactions = transactions.AsReadOnly();
            }
            catch
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
