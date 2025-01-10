using System.Linq.Expressions;
using AdventureWorksAPI.Models.DMO;

public static class ExpressionHelper
{
    public static Expression<Func<Product, bool>> BuildWhereExpression(ProductFilterModel filter)
    {
        var parameter = Expression.Parameter(typeof(Product), "p");

        // Allways
        var subcategoryNotNull = Expression.NotEqual(Expression.Property(parameter, "ProductSubcategory"), Expression.Constant(null));
        var standardCostGreaterThanZero = Expression.GreaterThan(Expression.Property(parameter, "StandardCost"), Expression.Constant(0m));

        Expression combinedExp = Expression.AndAlso(subcategoryNotNull, standardCostGreaterThanZero);

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
        if (!string.IsNullOrEmpty(filter.SearchString))
        {
            var searchProperty = Expression.Property(parameter, "Name");
            var searchConstant = Expression.Constant(filter.SearchString);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var searchCondition = Expression.Call(searchProperty, containsMethod, searchConstant);
            combinedExp = Expression.AndAlso(combinedExp, searchCondition);
        }

        return Expression.Lambda<Func<Product, bool>>(combinedExp, parameter);
    }
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
            "DateAsc" => query => query.OrderBy(p => p.SellStartDate),
            "DateDesc" => query => query.OrderByDescending(p => p.SellStartDate),
            _ => null
        };
    }
    public static Func<IQueryable<Product>, IQueryable<Product>> BuildPaginationExpression(int pageNumber, int pageSize)
    {
        return query => query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
    public static Expression<Func<T, bool>> CombineExpressions<T>(params Expression<Func<T, bool>>[] expressions)
    {
        if (expressions == null || expressions.Length == 0)
            throw new ArgumentException("At least one expression is required");

        var parameter = Expression.Parameter(typeof(T), "x");

        Expression combinedBody = null;
        foreach (var exp in expressions)
        {
            var body = Expression.Invoke(exp, parameter); // Invoke the original expression
            combinedBody = combinedBody == null ? body : Expression.AndAlso(combinedBody, body); // Combine with AndAlso
        }

        return Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
    }
}