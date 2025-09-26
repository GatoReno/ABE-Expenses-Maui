using AbeXP.Interfaces;
using AbeXP.Platforms.Android.Widget;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace AbeXP.Platforms.Android
{
    public class AddItemPagerAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        private readonly IExpenseRepository _expenseRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly int[] _layouts;

        public AddItemPagerAdapter(Context c, IExpenseRepository expenseRepository, ILoanRepository loanRepository)
        {
            _context = c;
            _expenseRepository = expenseRepository;
            _loanRepository = loanRepository;

            _layouts = new[]
            {
                Resource.Layout.expense_tab,
                Resource.Layout.loan_tab
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var _inflater = LayoutInflater.From(_context);
            var view = _inflater.Inflate(_layouts[viewType], parent, false);

            return viewType switch
            {
                0 => new ExpenseViewHolder(view, _expenseRepository),
                1 => new LoanViewHolder(view, _loanRepository),
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
