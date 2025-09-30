using AbeXP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Extensions
{
    public static class TagModelExtensions
    {

        public static IEnumerable<TagModelItem> ToTagModelItemList(this IEnumerable<TagModel> tags)
        {
            return tags.Select(pm => new TagModelItem(pm));
        }

        public static List<string> ToLocalizeList(this IEnumerable<TagModel> tags)
        {
            return tags.Select(pm => TagModelLocalizer.GetTagName(pm.Name)).ToList();
        }
    }
}
