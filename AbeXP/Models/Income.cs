using AbeXP.Interfaces;
using AbeXP.Common.Enum;
using System;
using System.Collections.Generic;

namespace AbeXP.Models
{
    public class Income : BaseUserOwnerEntity, IAmTransaction
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? PaymentTypeId { get; set; }
        public List<string> TagIds { get; set; } = new();

        public TransactionType Type => TransactionType.Income;
        public DateTime GetIndexDate() => Date;
        public TransactionModel ToTransactionModel() => new TransactionModel
        {
            Id = Id,
            UserId = UserId,
            InsertedDate = InsertedDate,
            Type = TransactionType.Income,
            Date = Date,
            Amount = Amount,
            Description = Description,
            PaymentTypeId = PaymentTypeId,
            TagIds = TagIds,
            DateIndex = Date
        };
    }
}
