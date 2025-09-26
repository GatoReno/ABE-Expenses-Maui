using AbeXP.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class ExpendituresRequest
    {
        public DateTime StartAt { get; set; } = DateTime.Now.FirstDayOfCurrentMonth();
        public DateTime EndAt { get; set; } = DateTime.Now.LastDayOfCurrentMonth();
        public int? LimitTo { get; set; }
    }
}
