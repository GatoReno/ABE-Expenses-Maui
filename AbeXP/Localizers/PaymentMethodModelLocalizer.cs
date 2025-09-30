using AbeXP.Resources.Strings;

namespace AbeXP.Localizers
{
    public class PaymentMethodModelLocalizer
    {
        public static string GetName(string resourceName)
        {
            var key = $"PaymentMethod_{resourceName}";
            return AppResources.ResourceManager.GetString(key) ?? resourceName;
        }
    }
}
