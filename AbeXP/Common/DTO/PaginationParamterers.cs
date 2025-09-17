using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Common.DTO
{
    public record PaginationParamterers
    {
        public PaginationParamterers()
        {

        }
        public PaginationParamterers(bool allRecords)
        {
            AllRecords = allRecords;
        }

        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public bool AllRecords { get; set; }

        public int? Skip => (PageNumber != null && PageSize != null) ? (PageNumber - 1) * PageSize : 0;
    }
}
