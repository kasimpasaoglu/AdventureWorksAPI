public class ProductFilterModel
{
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<string>? SelectedColors { get; set; }
    public string? SortBy { get; set; }
    public int PageSize { get; set; } = 12;
    public int PageNumber { get; set; } = 1;
}
