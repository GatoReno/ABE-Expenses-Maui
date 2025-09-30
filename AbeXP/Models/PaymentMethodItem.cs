using AbeXP.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class PaymentMethodItem : PaymentMethod
    {
        public PaymentMethodItem(PaymentMethod paymentMethod)
        {
            Id = paymentMethod.Id;
            Name = PaymentMethodLocalizer.GetPaymentMethodName(paymentMethod.Name);
            Details = paymentMethod.Details;
            InsertedDate = paymentMethod.InsertedDate;
        }


    }

    public class PaymentMethodLocalizer
    {
        public static string GetPaymentMethodName(string resourceName)
        {
            var key = $"PaymentMethod_{resourceName}";
            return AppResources.ResourceManager.GetString(key) ?? resourceName;
        }
    }
}
