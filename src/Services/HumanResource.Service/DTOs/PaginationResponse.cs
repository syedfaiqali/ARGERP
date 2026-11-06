namespace HumanResource.Service.DTOs
{
    public sealed record PaginationResponse<T>
    {
        public PaginationResponse() { }

        public PaginationResponse(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public IEnumerable<T> Items { get; init; } = Array.Empty<T>();
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
    }
}
