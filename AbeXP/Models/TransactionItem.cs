using AbeXP.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class TransactionItem
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public string IsPaid { get; set; }
        public string TypeDescription { get; set; } //Expense or Loan
        public TransactionType Type { get; set; }
        public string Icon { get; set; }
    }
}
