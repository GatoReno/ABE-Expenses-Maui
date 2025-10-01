using AbeXP.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class TagModel : BaseEntity
    {
        public TagModel()
        {
            
        }

        public TagModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string ColorHex { get; set; }
        
    }
}
