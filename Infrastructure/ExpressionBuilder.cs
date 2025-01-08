using System.Linq.Expressions;
using AdventureWorksAPI.Models.DMO;

public static class ExpressionBuilder
{
    /// <summary>
    /// Builds where section of querry
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static Expression<Func<Product, bool>> BuildWhereExpression(ProductFilterModel filter)
    {
        var parameter = Expression.Parameter(typeof(Product), "p");
        Expression combinedExp = Expression.Constant(true); // Başlangıç noktası (true)

        // Allways
        var subcategoryNotNull = Expression.NotEqual(Expression.Property(parameter, "ProductSubcategory"), Expression.Constant(null));
        var standardCostGreaterThanZero = Expression.GreaterThan(Expression.Property(parameter, "StandardCost"), Expression.Constant(0m));


        combinedExp = Expression.AndAlso(subcategoryNotNull, standardCostGreaterThanZero);

        // ProductCategoryId
        if (filter.ProductCategoryId.HasValue)
        {
            var subcategoryProperty = Expression.Property(Expression.Property(parameter, "ProductSubcategory"), "ProductCategoryId");
            var categoryValue = Expression.Constant(filter.ProductCategoryId.Value);
            var categoryCondition = Expression.Equal(subcategoryProperty, categoryValue);
            combinedExp = Expression.AndAlso(combinedExp, categoryCondition);

            // ProductSubcategoryId
            if (filter.ProductSubcategoryId.HasValue)
            {
                var subcategoryIdProperty = Expression.Property(parameter, "ProductSubcategoryId");
                var subcategoryIdValue = Expression.Constant(filter.ProductSubcategoryId.Value);
                var subcategoryCondition = Expression.Equal(subcategoryIdProperty, subcategoryIdValue);
                combinedExp = Expression.AndAlso(combinedExp, subcategoryCondition);
            }
        }


        // MinPrice
        if (filter.MinPrice.HasValue)
        {
            var standardCostProperty = Expression.Property(parameter, "StandardCost");
            var minPriceValue = Expression.Constant(filter.MinPrice.Value);
            var minPriceCondition = Expression.GreaterThanOrEqual(standardCostProperty, minPriceValue);
            combinedExp = Expression.AndAlso(combinedExp, minPriceCondition);
        }

        // MaxPrice
        if (filter.MaxPrice.HasValue)
        {
            var standardCostProperty = Expression.Property(parameter, "StandardCost");
            var maxPriceValue = Expression.Constant(filter.MaxPrice.Value);
            var maxPriceCondition = Expression.LessThanOrEqual(standardCostProperty, maxPriceValue);
            combinedExp = Expression.AndAlso(combinedExp, maxPriceCondition);
        }

        // SelectedColors
        if (filter.SelectedColors != null && filter.SelectedColors.Any())
        {
            var colorProperty = Expression.Property(parameter, "Color");
            var colorsConstant = Expression.Constant(filter.SelectedColors);
            var colorsCondition = Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Contains),
                new[] { typeof(string) },
                colorsConstant,
                colorProperty
            );
            combinedExp = Expression.AndAlso(combinedExp, colorsCondition);
        }

        return Expression.Lambda<Func<Product, bool>>(combinedExp, parameter);
    }

    /// <summary>
    /// Builds Orderby Section of querry
    /// </summary>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    public static Func<IQueryable<Product>, IOrderedQueryable<Product>>? BuildOrderByExpression(string? sortBy)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return null;
        }

        return sortBy switch
        {
            "PriceAsc" => query => query.OrderBy(p => p.StandardCost),
            "PriceDesc" => query => query.OrderByDescending(p => p.StandardCost),
            "NameAsc" => query => query.OrderBy(p => p.Name),
            "NameDesc" => query => query.OrderByDescending(p => p.Name),
            _ => null
        };
    }

    /// <summary>
    /// Builds Skip and Take Secion of querry for pagination
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static Func<IQueryable<Product>, IQueryable<Product>> BuildPaginationExpression(int pageNumber, int pageSize)
    {
        return query => query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

}