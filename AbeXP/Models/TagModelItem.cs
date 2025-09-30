using AbeXP.Localizers;

namespace AbeXP.Models
{
    public class TagModelItem : TagModel
    {
        public TagModelItem(TagModel tag)
        {
            Id = tag.Id;
            Name = tag.Name;
            ColorHex = tag.ColorHex;
            InsertedDate = tag.InsertedDate;
        }
        public bool IsSelected { get; set; } = false; // Para la selección en la UI
    }
}
