using AbeXP.Common.Constants;
using AbeXP.Common.Enum;

namespace AbeXP.Models
{
    public class TransactionModel : BaseUserOwnerEntity
    {
        public TransactionType Type { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? PaymentTypeId { get; set; }
        public List<string>? TagIds { get; set; }
        public virtual string UserId_Date => $"{UserId}_{InsertedDate.ToString(DateConstants.IndexDateFormat)}";
    }
}
