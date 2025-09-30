using AbeXP.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class TagModelItem : TagModel
    {
        public TagModelItem(TagModel tag)
        {
            Id = tag.Id;
            Name = TagModelLocalizer.GetTagName(tag.Name);
            ColorHex = tag.ColorHex;
            InsertedDate = tag.InsertedDate;
        }
        public bool IsSelected { get; set; } = false; // Para la selección en la UI
    }

    public class TagModelLocalizer
    {
        public static string GetTagName(string resourceName)
        {
            var key = $"Tag_{resourceName}";
            return AppResources.ResourceManager.GetString(key) ?? resourceName;
        }
    }
}
