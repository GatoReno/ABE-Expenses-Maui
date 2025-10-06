using AbeXP.Common.Constants;

namespace AbeXP.Models
{
    internal class IncomeIndexed : Income
    {
        public IncomeIndexed(Income income)
        {
            Id = income.Id;
            Date = income.Date;
            Amount = income.Amount;
            Description = income.Description;
            PaymentTypeId = income.PaymentTypeId;
            TagIds = income.TagIds;
            UserId = income.UserId;
            InsertedDate = income.InsertedDate;
        }

        public string UserId_Date => $"{UserId}_{Date.ToString(DateConstants.IndexDateFormat)}";
    }
}
