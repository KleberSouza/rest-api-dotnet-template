namespace rest_api_dotnet_template.Models
{
    public class PagedResultDto<TEntity>
    {
        public IEnumerable<TEntity> Items { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
