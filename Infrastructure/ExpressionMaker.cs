using System.Linq.Expressions;

public static class ExpressionMaker
{
    public static Expression<Func<T, bool>> Make<T>(T model) where T : class
    {
        var props = model.GetType().GetProperties();
        var parameter = Expression.Parameter(typeof(T), "s");
        Expression result = null;

        foreach (var prop in props)
        {
            var value = prop.GetValue(model);
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                continue;
            }
            // null degil ama bir deger tanimlanmamis, yani default deger ile gelmisse
            var defaultValue = Activator.CreateInstance(prop.PropertyType);
            if (object.Equals(value, defaultValue))
            {
                continue;
            }

            var propAccess = Expression.Property(parameter, prop.Name);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(propAccess, constant);

            result = result == null ? equality : Expression.AndAlso(result, equality);
        }

        return Expression.Lambda<Func<T, bool>>(result, parameter);
    }
}