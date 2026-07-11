namespace InsuranceApi.DTO
{
    public class PagedResponseDto<T>
    {
        public IEnumerable<T> Records { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
    
    }
}
