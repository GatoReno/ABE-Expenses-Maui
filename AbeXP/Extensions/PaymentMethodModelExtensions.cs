using AbeXP.Localizers;
using AbeXP.Models;

namespace AbeXP.Extensions
{
    public static class PaymentMethodModelExtensions
    {

        public static List<PaymentMethodModelItem> ToPaymentMethodItemList(this IEnumerable<PaymentMethod> tags)
        {
            return tags.Select(pm => new PaymentMethodModelItem(pm)).ToList();
        }
        public static List<PaymentMethod> ToLocalizedList(this IEnumerable<PaymentMethod> paymentMethods)
        {
            return paymentMethods.Select(pm => pm.ToLocalizedModel()).ToList();
        }

        public static PaymentMethod ToLocalizedModel(this PaymentMethod paymentMethod)
        {
            paymentMethod.Name = PaymentMethodModelLocalizer.GetName(paymentMethod.Name);

            return paymentMethod;
        }

    }
}
