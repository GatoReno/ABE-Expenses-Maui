using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class IndexItemRequest
    {
        public string OrderBy { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public int? LimitTo { get; set; }
    }
}
