using AbeXP.Interfaces;
using AbeXP.Platforms.Android.Widget.ViewHolders;
using AbeXP.UseCases.Interfaces;
using AbeXP.UseCases.Plugins;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace AbeXP.Platforms.Android.Widget
{
    public class AddItemPagerAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        private readonly ICreateExpenseUseCase _createExpenseUseCase;
        private readonly ICreateIncomeUseCase _createIncomeUseCase;
        private readonly IGetTransactionCatalogsUseCase _getTransactionCatalogsUseCase;
        private readonly IWidgetUpdater _widgetUpdater;
        private readonly int[] _layouts;

        public AddItemPagerAdapter(Context c, ICreateExpenseUseCase createExpenseUseCase, ICreateIncomeUseCase createIncomeUseCase, IGetTransactionCatalogsUseCase getTransactionCatalogsUseCase, IWidgetUpdater widgetUpdater)
        {
            _context = c;
            _createExpenseUseCase = createExpenseUseCase;
            _createIncomeUseCase = createIncomeUseCase;
            _getTransactionCatalogsUseCase = getTransactionCatalogsUseCase;
            _widgetUpdater = widgetUpdater;
            _layouts = new[]
            {
                Resource.Layout.expense_tab,
                Resource.Layout.income_tab,
                Resource.Layout.loan_tab
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var _inflater = LayoutInflater.From(_context);
            var view = _inflater.Inflate(_layouts[viewType], parent, false);

            return viewType switch
            {
                0 => new ExpenseViewHolder(view, _createExpenseUseCase, _getTransactionCatalogsUseCase, _widgetUpdater),
                1 => new IncomeViewHolder(view, _createIncomeUseCase, _getTransactionCatalogsUseCase, _widgetUpdater),
                _ => throw new ArgumentOutOfRangeException(nameof(viewType))
            };
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
        
        }

        public override int GetItemViewType(int position) => position;
        public override int ItemCount => _layouts.Length;
    }
}
