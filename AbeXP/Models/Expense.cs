using AbeXP.Interfaces;
using AbeXP.Common.Enum;
using AbeXP.Attributes;
using System;
using AbeXP.Common.Constants;
namespace AbeXP.Models
{
    public class Expense : TransactionModel, IAmTransaction
    {
        public Expense()
        {
            Type = TransactionType.Expense;
        }

        public Expense(TransactionModel transaction)
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
