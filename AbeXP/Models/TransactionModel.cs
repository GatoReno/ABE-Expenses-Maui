using AbeXP.Common.Constants;
using AbeXP.Common.Enum;

namespace AbeXP.Models
{
    public class TransactionModel : BaseUserOwnerEntity
    {
        public TransactionType Type { get; set; }

        // Common
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? PaymentTypeId { get; set; }
        public List<string>? TagIds { get; set; }

        // Loan-specific
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateGiven { get; set; }
        public DateTime? SuggestedPaybackDate { get; set; }
        public bool? IsPaid { get; set; }
        public string? Notes { get; set; }

        // Indexing
        public DateTime DateIndex { get; set; } = DateTime.Now;
        public string UserId_Date => $"{UserId}_{DateIndex.ToString(DateConstants.IndexDateFormat)}";
    }
}
