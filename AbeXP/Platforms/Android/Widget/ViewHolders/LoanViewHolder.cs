using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.Resources.Strings;
using AbeXP.UseCases.Interfaces;
using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.CheckBox;
using Google.Android.Material.TextField;
using View = Android.Views.View;

namespace AbeXP.Platforms.Android.Widget.ViewHolders
{
    internal class LoanViewHolder : ExpenditureBaseViewHolder
    {
        private readonly ICreateLoanUseCase _createLoanUseCase;
        private DateTime dateGiven = DateTime.Now;
        private DateTime datePayment = DateTime.Now.AddDays(10);

        public LoanViewHolder(View itemView, ICreateLoanUseCase createLoanUseCase, IWidgetUpdater widgetUpdater) : base(itemView, widgetUpdater)
        {
            _createLoanUseCase = createLoanUseCase;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            SetupLocalization();
            SetupDatePickers();
            SetupSaveButton();
        }

        private void SetupLocalization()
        {
            // Set hints from resources
            var edtPersonName = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytLoanPersonName);
            edtPersonName.Hint = AppResources.FullName; // "Nombre completo"

            var edtEmail = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytLoanEmail);
            edtEmail.Hint = AppResources.Email; // "Correo"

            var edtAmount = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytLoanAmount);
            edtAmount.Hint = AppResources.Amount; // "Monto"

            var edtLoanDate = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytLoanDate);
            edtLoanDate.Hint = AppResources.DateGiven; // "Fecha del préstamo"

            var edtPaymentDate = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytPaymentDate);
            edtPaymentDate.Hint = AppResources.SuggestedPaymentDate; // "Fecha sugerida de pago"

            var chkIsPaid = ItemView.FindViewById<MaterialCheckBox>(Resource.Id.chkIsPaid);
            chkIsPaid.Text = AppResources.IsPaid; // "¿Ya está pagado?"

            var edtNotes = ItemView.FindViewById<TextInputLayout>(Resource.Id.lytLoanNotes);
            edtNotes.Hint = AppResources.Note; // "Nota"

            var btnSave = ItemView.FindViewById<MaterialButton>(Resource.Id.btnSaveLoan);
            btnSave.Text = AppResources.Save; // "Guardar"
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
                    await _createLoanUseCase.ExecuteAsync(loan);

                    NotifyWidgetUpdate();
                    Toast.MakeText(ItemView.Context, AppResources.Success, ToastLength.Short).Show();

                    ResetForm();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(ItemView.Context, AppResources.ErrorWhileProcessingRequest, ToastLength.Short).Show();
                }
            }
        }

        private void ResetForm()
        {
            dateGiven = DateTime.Now;
            datePayment = DateTime.Now;

            var edtPersonName = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtLoanPersonName);
            var edtEmail = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtLoanEmail);
            var edtAmount = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtLoanAmount);
            var edtLoanDate = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtLoanDate);
            var edtPaymentDate = ItemView.FindViewById<TextInputEditText>(Resource.Id.edtPaymentDate);
            var edtIsPaid = ItemView.FindViewById<MaterialCheckBox>(Resource.Id.chkIsPaid);
            var edtNotes = ItemView.FindViewById<TextInputEditText>(Resource.Id.txtLoanNotes);


            edtPersonName.Text = string.Empty;
            edtEmail.Text = string.Empty;
            edtAmount.Text = string.Empty;
            edtLoanDate.Text = dateGiven.ToString(DateConstants.WidgetDateFormat);
            edtPaymentDate.Text = datePayment.ToString(DateConstants.WidgetDateFormat);
            edtIsPaid.Checked = false;
            edtNotes.Text = string.Empty;
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
