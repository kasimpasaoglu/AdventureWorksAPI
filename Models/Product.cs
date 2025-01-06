public class ProductVM
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal ListPrice { get; set; }
    public string Color { get; set; }
    public decimal StandardCost { get; set; }
    public string Description { get; set; }
    public string LargePhoto { get; set; }
}

public class ProductDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal ListPrice { get; set; }
    public string Color { get; set; }
    public decimal StandardCost { get; set; }
    public string Description { get; set; }
    public string LargePhoto { get; set; }
}