using AbeXP.Common.Constants;
using AbeXP.Interfaces;
using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AbeXP.ViewModels

{
    public partial class LoanFormViewModel : ObservableObject
    {
        private readonly ICreateLoanUseCase _loanUseCase;
        private readonly INavigationService _navigation;

        public LoanFormViewModel(ICreateLoanUseCase loanUseCase, INavigationService navigation)
        {
            _loanUseCase = loanUseCase;
            _navigation = navigation;
        }

        #region PROPERTIES
        [ObservableProperty] private string _personName;
        [ObservableProperty] private string _email;
        [ObservableProperty] private decimal _amount;
        [ObservableProperty] private DateTime _dateGiven = DateTime.Now;
        [ObservableProperty] private DateTime? _suggestedPaybackDate;
        [ObservableProperty] private bool _isPaid;
        [ObservableProperty] private string _notes;
        #endregion

        [RelayCommand]
        private async Task SaveLoan()
        {
            try
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

                var result = await _loanUseCase.ExecuteAsync(loan);
                if (result.IsSuccessful)
                {
                    App.Alert.ShowToast("Loan saved successfully.");

                    var routePramteres = new ShellNavigationQueryParameters()
                    {
                        { NavigationConstants.TRIGGER_DASHBOARD_PARAM, true }
                    };
                    await _navigation.PopAsync(routePramteres);
                }
            }
            catch (Exception ex)
            {
                App.Alert.ShowAlert("Error", "Could not save data.");
            }
        }
    }
}

