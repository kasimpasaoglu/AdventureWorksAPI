using System;
using System.Linq.Expressions;

public static class ExpressionMaker
{
    public static Expression<Func<T, bool>> Make<T>(T model) where T : class
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Model cannot be null.");
        }

        var props = model.GetType().GetProperties();
        var parameter = Expression.Parameter(typeof(T), "s");
        Expression result = null;

        foreach (var prop in props)
        {
            var value = prop.GetValue(model);

            // Null veya boş değerleri atla
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                continue;
            }

            // Nullable türler ve enum'lar için temel tipi belirle
            var propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            // Enum türlerini uygun türe dönüştür
            if (propertyType.IsEnum)
            {
                value = Enum.ToObject(propertyType, value);
            }
            else
            {
                // Sabit değeri uygun türe dönüştür
                value = Convert.ChangeType(value, propertyType);
            }

            // Özelliğe erişim ve sabit değer tanımlaması
            var propAccess = Expression.Property(parameter, prop.Name);
            var constant = Expression.Constant(value, prop.PropertyType);

            // Eşitlik ifadesini oluştur ve birleştir
            var equality = Expression.Equal(propAccess, constant);
            result = result == null ? equality : Expression.AndAlso(result, equality);
        }

        if (result == null)
        {
            throw new InvalidOperationException("No valid properties found to create an expression.");
        }

        return Expression.Lambda<Func<T, bool>>(result, parameter);
    }
}
