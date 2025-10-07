using AbeXP.Common.Enum;
using AbeXP.Attributes;
using System;
using AbeXP.Common.Constants;
namespace AbeXP.Models
{
    public class ExpenseTransactionModel : TransactionModel
    {
        public ExpenseTransactionModel()
        {
            Type = TransactionType.Expense;
        }

        public ExpenseTransactionModel(TransactionModel transaction)
        {
            Type = transaction.Type;
            Date = transaction.Date;
            Amount = transaction.Amount;
            Description = transaction.Description;
            PaymentTypeId = transaction.PaymentTypeId;
            TagIds = transaction.TagIds;
            UserId = transaction.UserId;
            InsertedDate = transaction.InsertedDate;
        }
    }
}
