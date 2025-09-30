using AbeXP.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Localizers
{
    public class TagModelLocalizer
    {
        public static string GetTagName(string resourceName)
        {
            var key = $"Tag_{resourceName}";
            return AppResources.ResourceManager.GetString(key) ?? resourceName;
        }
    }
}
