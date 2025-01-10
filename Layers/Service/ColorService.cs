using System.Linq.Expressions;
using AdventureWorksAPI.Models.DMO;
using AutoMapper;

public interface IColorService
{
    string[] GetColors(int? categoryId, int? subCategoryId);
}

public class ColorService : IColorService
{
    private IUnitOfWork _unitOfWork;
    public ColorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public string[] GetColors(int? categoryId, int? subCategoryId)
    {
        // Başlangıç filtresi: Color boş olmamalı
        Expression<Func<Product, bool>> combinedExpression = x => !string.IsNullOrEmpty(x.Color);

        // Eğer categoryId verilmişse, filtreye ekle
        if (categoryId.HasValue && categoryId > 0)
        {
            combinedExpression = ExpressionHelper.CombineExpressions(
                combinedExpression,
                x => x.ProductSubcategory.ProductCategoryId == categoryId
            );

            // Eğer subCategoryId verilmişse, alt kategori filtresini de ekle
            if (subCategoryId.HasValue && subCategoryId > 0)
            {
                combinedExpression = ExpressionHelper.CombineExpressions(
                    combinedExpression,
                    x => x.ProductSubcategoryId == subCategoryId
                );
            }
        }

        // Renkleri seçmek için selector
        Expression<Func<Product, string>> selector = p => p.Color;

        // Sorguyu çalıştır ve sonuçları işle
        var colors = _unitOfWork.Product
            .Find(combinedExpression, selector, distinct: true)
            .Result
            .OrderBy(x => x) // Alfabetik sıralama
            .ToArray();

        return colors;
    }


}