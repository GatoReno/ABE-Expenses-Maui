using AbeXP.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Localizers
{
    internal class TransactionModelLocalizer
    {
        public static string GetTypeName(string resourceName)
        {
            var key = $"TransactionType_{resourceName}";
            return AppResources.ResourceManager.GetString(key) ?? resourceName;
        }
    }
}
