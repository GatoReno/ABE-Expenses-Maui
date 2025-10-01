using AbeXP.Localizers;
using AbeXP.Models;

namespace AbeXP.Extensions
{
    public static class TagModelExtensions
    {

        public static List<TagModelItem> ToTagModelItemList(this IEnumerable<TagModel> tags)
        {
            return tags.Select(pm => new TagModelItem(pm)).ToList();
        }

        public static List<string> ToLocalizeStringList(this IEnumerable<TagModel> tags)
        {
            return tags.Select(pm => pm.ToLocalizedModel().Name).ToList();
        }

        public static List<TagModel> ToLocalizeList(this IEnumerable<TagModel> tags)
        {
            return tags.Select(pm => pm.ToLocalizedModel()).ToList();
        }

        public static TagModel ToLocalizedModel(this TagModel tag)
        {
            tag.Name = TagModelLocalizer.GetTagName(tag.Name);

            return tag;
        }
    }
}
