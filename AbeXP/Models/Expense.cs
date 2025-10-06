using AbeXP.Interfaces;
using AbeXP.Common.Enum;
using AbeXP.Attributes;
using System;
namespace AbeXP.Models
{
    public class Expense : TransactionModel, IAmTransaction
    {

        public Expense()
        {
            Type = TransactionType.Expense;
        }

        public DateTime GetIndexDate() => Date;
        //public TransactionModel ToTransactionModel() => new TransactionModel
        //{
        //    Id = Id,
        //    UserId = UserId,
        //    InsertedDate = InsertedDate,
        //    Type = TransactionType.Expense,
        //    Date = Date,
        //    Amount = Amount,
        //    Description = Description,
        //    PaymentTypeId = PaymentTypeId,
        //    TagIds = TagIds,
        //    DateIndex = Date
        //};
    }

    public class PaymentMethod : BaseEntity
    {
        public string Name { get; set; } = ""; // Ej: "Tarjeta", "Crédito", "Transferencia"
        public string Details { get; set; } = ""; // Ej: número de tarjeta, banco, etc.
    }
}
