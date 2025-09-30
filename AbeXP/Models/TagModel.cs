using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class TagModel : BaseEntity
    {
        public string Name { get; set; } = "";
        public string ColorHex { get; set; } = "#FFFFFF";
        public bool IsSelected { get; set; } = false; // Para la selección en la UI
    }
}
