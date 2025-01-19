public class ProductFilterModel
{
    public int? ProductCategoryId { get; set; }
    public int? ProductSubcategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<string>? SelectedColors { get; set; }
    public string? SortBy { get; set; }
    public string? SearchString { get; set; }
    public int PageSize { get; set; } = 12;
    public int PageNumber { get; set; } = 1;
}

// her bir prop icin hangi expressionun kullanilmasi gerektigini bir sekilde get set metodlarina verilebiliyor olmasi lazim
// bunu bir arastir
