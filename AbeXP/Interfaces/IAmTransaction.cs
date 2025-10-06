using AbeXP.Common.Enum;

namespace AbeXP.Interfaces
{
    public interface IAmTransaction
    {
        TransactionType Type { get; }
        DateTime GetIndexDate();
        AbeXP.Models.TransactionModel ToTransactionModel();
    }
}
