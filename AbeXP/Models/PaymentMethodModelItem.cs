using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class PaymentMethodModelItem : PaymentMethod
    {
        public PaymentMethodModelItem(PaymentMethod paymentMethod)
        {
            Id = paymentMethod.Id;
            Name = paymentMethod.Name;
            Details = paymentMethod.Details;
            InsertedDate = paymentMethod.InsertedDate;
        }

        public override string ToString() => Name;
    }
}
