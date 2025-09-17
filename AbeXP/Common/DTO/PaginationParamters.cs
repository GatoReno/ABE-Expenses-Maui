namespace AbeXP.Common.DTO
{
    public record PaginationParamters
    {
        public PaginationParamters()
        {

        }
        public PaginationParamters(bool allRecords)
        {
            AllRecords = allRecords;
        }

        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public bool AllRecords { get; set; }

        public int? Skip => (PageNumber != null && PageSize != null) ? (PageNumber - 1) * PageSize : 0;
    }
}
