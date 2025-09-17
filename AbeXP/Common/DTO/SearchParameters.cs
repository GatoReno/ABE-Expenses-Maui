namespace AbeXP.Common.DTO
{
    public record SearchParameters
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchKeyWords { get; set; }
        public PaginationParamters? Pagination { get; set; }
    }
}
