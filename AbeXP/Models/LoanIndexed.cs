using AbeXP.Common.Constants;
namespace AbeXP.Models
{
    internal class LoanIndexed : Loan
    {
        public LoanIndexed(Loan loan) 
        {
            Id = loan.Id;
            PersonName = loan.PersonName;
            Email = loan.Email;
            Amount = loan.Amount;
            DateGiven = loan.DateGiven;
            SuggestedPaybackDate = loan.SuggestedPaybackDate;
            IsPaid = loan.IsPaid;
            Notes = loan.Notes;
            UserId = loan.UserId;
            InsertedDate = loan.InsertedDate;
        }

        public string UserId_DateGiven => $"{UserId}_{DateGiven.ToString(DateConstants.IndexDateFormat)}";
    }
}
