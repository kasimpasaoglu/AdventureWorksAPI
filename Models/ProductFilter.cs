public class ProductFilterModel
{
    [FilterAttribute("ProductId")]
    public int? CategoryId { get; set; }
    public int? ProductSubcategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<string>? SelectedColors { get; set; }
    public string? SortBy { get; set; }
    public int PageSize { get; set; } = 12;
    public int PageNumber { get; set; } = 1;
}
