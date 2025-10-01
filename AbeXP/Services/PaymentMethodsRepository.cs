using AbeXP.Interfaces;
using AbeXP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Services
{
    public class PaymentMethodsRepository : FibRepository<PaymentMethod>, IPaymentMethodsRepository
    {
        public IFibInstance _db { get; set; }
        public PaymentMethodsRepository(IFibInstance fibInstance, string collection)
            : base(fibInstance, collection)
        {
            _db = fibInstance;
        }
    }
}
