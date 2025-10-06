using AbeXP.Common.Enum;
using AbeXP.Resources.Strings;
using AbeXP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class TransactionItem
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }
        public string TypeDescription { get; set; } //Expense or Loan
        public TransactionType Type { get; set; }
        public string Icon { get; set; }
    }
}
