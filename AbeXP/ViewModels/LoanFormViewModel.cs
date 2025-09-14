using System;
using AbeXP.Models;
using Android.Accounts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AbeXP.ViewModels

{
    public partial class LoanFormViewModel : ObservableObject
    {
        [ObservableProperty] private string personName;
        [ObservableProperty] private string email;
        [ObservableProperty] private decimal amount;
        [ObservableProperty] private DateTime dateGiven = DateTime.Now;
        [ObservableProperty] private DateTime? suggestedPaybackDate;
        [ObservableProperty] private bool isPaid;
        [ObservableProperty] private string notes;

        public LoanFormViewModel()
        {
        }

        [RelayCommand]
        private async Task SaveLoan()
        {
            // Aquí crearíamos el objeto Loan
            var loan = new Loan
            {
                PersonName = PersonName,
                Email = Email,
                Amount = Amount,
                DateGiven = DateGiven,
                SuggestedPaybackDate = SuggestedPaybackDate,
                IsPaid = IsPaid,
                Notes = Notes
            };

            // TODO: Guardar en base de datos o enviar a Firebase
            await Application.Current.MainPage.DisplayAlert("Éxito", "Préstamo registrado", "OK");
        }
    }
}

