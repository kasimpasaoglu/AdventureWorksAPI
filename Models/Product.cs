public class ProductVM
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal ListPrice { get; set; }
    public decimal StandardCost { get; set; }
    public string Color { get; set; }
    public int ProductCategoryId { get; set; }
    public int? ProductSubcategoryId { get; set; }
    public string Description { get; set; }
    public byte[] LargePhoto { get; set; }
}

public class ProductDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal ListPrice { get; set; }
    public decimal StandardCost { get; set; }
    public string Color { get; set; }
    public int ProductCategoryId { get; set; }
    public int? ProductSubcategoryId { get; set; }
    public string Description { get; set; }
    public byte[] LargePhoto { get; set; }
}