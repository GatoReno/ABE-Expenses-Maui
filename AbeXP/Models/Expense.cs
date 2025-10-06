using AbeXP.Interfaces;
using AbeXP.Common.Enum;
using AbeXP.Attributes;
using System;
namespace AbeXP.Models
{
    public class Expense : BaseUserOwnerEntity, IAmTransaction
    {
        public DateTime Date { get; set; } = DateTime.Now; // Fecha y hora del gasto
        public decimal Amount { get; set; } // Cantidad gastada
        public string Description { get; set; } = ""; // Detalles adicionales
        public string PaymentTypeId { get; set; } // Referencia al tipo de pago usado
        public List<string> TagIds { get; set; } = new(); // Referencia a etiquetas asociadas

        public TransactionType Type => TransactionType.Expense;
        public DateTime GetIndexDate() => Date;
        public TransactionModel ToTransactionModel() => new TransactionModel
        {
            Id = Id,
            UserId = UserId,
            InsertedDate = InsertedDate,
            Type = TransactionType.Expense,
            Date = Date,
            Amount = Amount,
            Description = Description,
            PaymentTypeId = PaymentTypeId,
            TagIds = TagIds,
            DateIndex = Date
        };
    }
    public class Tag
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = ""; // Ej: "Comida", "Ropa", "Transporte"
        public string ColorHex { get; set; } = "#FFFFFF"; // Opcional: para UI
    }

    public class PaymentMethod : BaseEntity
    {
        public string Name { get; set; } = ""; // Ej: "Tarjeta", "Crédito", "Transferencia"
        public string Details { get; set; } = ""; // Ej: número de tarjeta, banco, etc.
    }

    public class Loan : BaseUserOwnerEntity, IAmTransaction
    {
        public string PersonName { get; set; } = ""; // Nombre del deudor
        public string Email { get; set; } = ""; // Email del deudor (opcional)
        public decimal Amount { get; set; } // Cantidad prestada
        public DateTime DateGiven { get; set; } = DateTime.Now; // Fecha del préstamo
        public DateTime? SuggestedPaybackDate { get; set; } // Fecha sugerida de pago
        public bool IsPaid { get; set; } = false; // Estado del préstamo
        public string Notes { get; set; } = ""; // Detalles adicionales

        public TransactionType Type => TransactionType.Loan;
        public DateTime GetIndexDate() => DateGiven;
        public TransactionModel ToTransactionModel() => new TransactionModel
        {
            Id = Id,
            UserId = UserId,
            InsertedDate = InsertedDate,
            Type = TransactionType.Loan,
            Amount = Amount,
            PersonName = PersonName,
            Email = Email,
            DateGiven = DateGiven,
            SuggestedPaybackDate = SuggestedPaybackDate,
            IsPaid = IsPaid,
            Notes = Notes,
            Description = PersonName,
            DateIndex = DateGiven
        };
    }
}
