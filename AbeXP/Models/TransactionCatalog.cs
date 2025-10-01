using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class TransactionCatalog
    {
        public IReadOnlyList<PaymentMethod> PaymentMethods { get; set; }
        public IReadOnlyList<TagModel> Tags { get; set; }
    }
}
