using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Common.DTO
{
    public record SearchParameters
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchKeyWords { get; set; }
        public PaginationParamterers? Pagination { get; set; }
    }
}
