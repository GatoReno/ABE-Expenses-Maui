using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Models;
using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.CheckBox;
using Google.Android.Material.TextField;
using View = Android.Views.View;

namespace AbeXP.Platforms.Android.Widget.ViewHolders
{
    internal class LoanViewHolder : ExpenditureBaseViewHolder
    {
        private readonly ILoanRepository _loanRepository;
        private DateTime dateGiven = DateTime.Now;
        private DateTime datePayment = DateTime.Now.AddDays(10);

        public LoanViewHolder(View itemView, ILoanRepository loanRepository) : base(itemView)
        {
            _loanRepository = loanRepository;
            InitializeComponents();
        }

        private void InitializeComponents()
        {

            SetupDatePickers();
            SetupSaveButton();
        }

        private void SetupDatePickers()
        {
            var edtLoanDate = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtLoanDate);
            var edtPaymentDate = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtPaymentDate);

            edtLoanDate.Text = dateGiven.ToString(DateConstants.WidgetDateFormat);
            edtPaymentDate.Text = datePayment.ToString(DateConstants.WidgetDateFormat);

            edtLoanDate.Click += (s, e) => ShowDatePicker(ItemView.Context, edtLoanDate, (date) => dateGiven = date);
            edtPaymentDate.Click += (s, e) => ShowDatePicker(ItemView.Context, edtPaymentDate, (date) => datePayment = date);

        }

        private void SetupSaveButton()
        {
            var btnSave = ItemView.FindViewById<MaterialButton>(Resource.Id.btnSaveLoan);
            btnSave.Click -= BtnSave_Click;
            btnSave.Click += BtnSave_Click;

            // store position if needed
            async void BtnSave_Click(object sender, EventArgs e)
            {

                try
                {
                    var loan = MapLoan();
                    await _loanRepository.AddAsync(new LoanIndexed(loan));

                    NotifyWidgetUpdate();
                    Toast.MakeText(ItemView.Context, "Success", ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ItemView.Context, "Error while processing request", ToastLength.Short).Show();
                }
                finally
                {
                    await Task.Delay(1000);
                    FinishActivity();
                }

            }
        }

        private Loan MapLoan()
        {
            var loan = new Loan();

            var edtPersonName = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtLoanPersonName);
            var edtEmail = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtLoanEmail);
            var edtAmount = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtLoanAmount);
            var edtIsPaid = ItemView.FindViewById<MaterialCheckBox>(Resource.Id.chkIsPaid);
            var edtNotes = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtLoanNotes);

            loan.PersonName = edtPersonName.Text ?? string.Empty;
            loan.Email = edtEmail.Text ?? string.Empty;
            loan.IsPaid = edtIsPaid.Checked;
            loan.Notes = edtNotes.Text ?? string.Empty;
            loan.DateGiven = dateGiven;
            loan.SuggestedPaybackDate = datePayment;

            if (decimal.TryParse(edtAmount.Text, out var amount))
            {
                loan.Amount = amount;
            }

            return loan;
        }
    }
}
