using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    internal class ExpenditureItem
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } //Expense or Loan
    }
}
