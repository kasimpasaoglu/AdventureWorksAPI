public class ProductVM
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal ListPrice { get; set; }
    public decimal StandardCost { get; set; }
    public string Color { get; set; }
    public byte[] LargePhoto { get; set; }
}

public class ProductDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal ListPrice { get; set; }
    public decimal StandardCost { get; set; }
    public string Color { get; set; }
    public byte[] LargePhoto { get; set; }
}

public class DetailedProductVM
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal ListPrice { get; set; }
    public decimal StandardCost { get; set; }
    public string Color { get; set; }
    /// <summary>
    /// H = High, M = Medium, L = Low
    /// </summary>
    public string? Class { get; set; }
    /// <summary>
    /// W = Womens, M = Mens, U = Universal
    /// </summary>
    public string? Style { get; set; }
    public string? Size { get; set; }
    public int ProductCategoryId { get; set; }
    public int? ProductSubcategoryId { get; set; }
    public string Description { get; set; }
    public byte[] LargePhoto { get; set; }
}

public class DetailedProductDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal ListPrice { get; set; }
    public decimal StandardCost { get; set; }
    public string Color { get; set; }
    public string? Class { get; set; }
    public string? Style { get; set; }
    public string? Size { get; set; }
    public int ProductCategoryId { get; set; }
    public int? ProductSubcategoryId { get; set; }
    public string Description { get; set; }
    public byte[] LargePhoto { get; set; }

    // public DetailedProductVM ToVm()
    // {
    //     var model = new DetailedProductVM()
    //     {
    //         ProductId = this.ProductId,
    //         Name = this.Name


    //     };


    //     return model;
    // }
}