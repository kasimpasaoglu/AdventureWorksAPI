using System.Linq.Expressions;
using AdventureWorksAPI.Models.DMO;

public static class Constants
{
    public static string[] GetSimpleIncludes()
    {
        return
        [
            "ProductProductPhotos.ProductPhoto",
            "ProductSubcategory.ProductCategory"
        ];
    }
    public static string[] GetDetailedIncludes()
    {
        return
        [
            "ProductProductPhotos.ProductPhoto",
            "ProductModel.ProductModelProductDescriptionCultures.ProductDescription",
            "ProductSubcategory.ProductCategory"
        ];
    }
    public static Expression<Func<Product, DetailedProductDTO>> GetDetailedSelectors()
    {
        return p => new DetailedProductDTO
        {
            ProductId = p.ProductId,
            Name = p.Name,
            ListPrice = p.ListPrice,
            StandardCost = p.StandardCost,
            Color = p.Color,
            Class = p.Class,
            Style = p.Style,
            Size = p.Size,
            ProductCategoryId = p.ProductSubcategory.ProductCategoryId,
            ProductSubcategoryId = p.ProductSubcategoryId,
            Description = p.ProductModel.ProductModelProductDescriptionCultures.FirstOrDefault().ProductDescription.Description,
            LargePhoto = p.ProductProductPhotos.FirstOrDefault().ProductPhoto.LargePhoto
        };
    }
    public static Expression<Func<Product, ProductDTO>> GetSimpleSelectors()
    {
        return p => new ProductDTO
        {
            ProductId = p.ProductId,
            Name = p.Name,
            ListPrice = p.ListPrice,
            StandardCost = p.StandardCost,
            Color = p.Color,
            LargePhoto = p.ProductProductPhotos.FirstOrDefault().ProductPhoto.LargePhoto
        };
    }
}