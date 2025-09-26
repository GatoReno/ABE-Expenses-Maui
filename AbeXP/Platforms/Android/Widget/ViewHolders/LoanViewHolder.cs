using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.Platforms.Android.Widget.ViewHolders;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using View = Android.Views.View;

namespace AbeXP.Platforms.Android.Widget
{
    internal class LoanViewHolder : ExpenditureBaseViewHolder
    {
        private readonly IExpenseRepository _expenseRepository;
        private Loan loan = new Loan();

        public LoanViewHolder(View itemView, IExpenseRepository expenseRepository) : base(itemView)
        {
            _expenseRepository = expenseRepository;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            SetupDatePickers();
            SetupSaveBtn();
        }

        private void SetupDatePickers()
        {
            var edtLoanDate = ItemView.FindViewById<EditText>(Resource.Id.edtLoanDate);
            var edtPaymentDate = ItemView.FindViewById<EditText>(Resource.Id.edtPaymentDate);

            edtLoanDate.Click += (s, e) => ShowDatePicker(ItemView.Context, edtLoanDate, (date) => loan.DateGiven = date);
            edtPaymentDate.Click += (s, e) => ShowDatePicker(ItemView.Context, edtPaymentDate, (date) => loan.SuggestedPaybackDate = date);

        }

        private void SetupSaveBtn()
        {
            var btnSave = ItemView.FindViewById<MaterialButton>(Resource.Id.btnSaveLoan);
            btnSave.Click -= BtnSave_Click;
            btnSave.Click += BtnSave_Click;

            // store position if needed
            void BtnSave_Click(object sender, EventArgs e)
            {
                FinishActivity();
            }

        }
    }
}
