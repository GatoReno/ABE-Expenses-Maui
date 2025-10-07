using AbeXP.Common.Enum;
using System;
using System.Collections.Generic;

namespace AbeXP.Models
{
    public class IncomeTransactionModel : TransactionModel
    {
        public IncomeTransactionModel()
        {
            Type = TransactionType.Income;
        }

        public IncomeTransactionModel(TransactionModel transaction)
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
