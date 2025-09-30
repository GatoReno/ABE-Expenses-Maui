using AbeXP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Extensions
{
    public static class PaymentMethodModelExtensions
    {
        public static IEnumerable<PaymentMethodItem> ToPaymentMethodItemList(this IEnumerable<PaymentMethod> paymentMethods)
        {
            return paymentMethods.Select(pm => new PaymentMethodItem(pm));
        }

    }
}
