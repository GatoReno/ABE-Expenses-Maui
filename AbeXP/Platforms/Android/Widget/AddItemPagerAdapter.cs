using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.Platforms.Android.Widget;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Adapter;
using Google.Android.Material.Button;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;
using Java.Util.Zip;
using System.Collections;
using static Android.App.DatePickerDialog;
using static AndroidX.RecyclerView.Widget.RecyclerView;
using View = Android.Views.View;

namespace AbeXP.Platforms.Android
{
    public class AddItemPagerAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        private readonly IExpenseRepository _expenseRepository;
        private readonly int[] _layouts;

        public AddItemPagerAdapter(Context c, IExpenseRepository expenseRepository)
        {
            _context = c;
            _expenseRepository = expenseRepository;

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
                1 => new LoanViewHolder(view, _expenseRepository),
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
