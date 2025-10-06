using AbeXP.Interfaces;
using AbeXP.Common.Enum;
using System;
using System.Collections.Generic;

namespace AbeXP.Models
{
    public class Income : TransactionModel, IAmTransaction
    {
        public Income()
        {
            Type = TransactionType.Income;
        }

        public Income(TransactionModel transaction)
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
