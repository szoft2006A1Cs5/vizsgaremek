using System.Linq.Expressions;
using System.Reflection;
using backend.Contexts;
using backend.Serialization;

namespace backend.VisibilityFiltering;

public static class FilteredExpressionBuilder
{
    public static Expression<Func<T, object>>? BuildFilteredExpression<T>(Context ctx, VisibilityLevel level)
    {
        var model = Expression.Parameter(typeof(T), "model");
        var bindings = new List<MemberBinding>();

        var modelProps = ctx.Model.FindEntityType(typeof(T));

        if (modelProps == null) return null;

        foreach (var prop in modelProps.GetProperties())
        {
            var propVisibility = prop.PropertyInfo?.GetCustomAttribute<VisibleToAttribute>()?
                                .VisibilityLevel ?? VisibilityLevel.Public;

            if (level < propVisibility || prop.PropertyInfo == null) continue;

            var expNode = Expression.Property(model, prop.PropertyInfo);
            bindings.Add(Expression.Bind(prop.PropertyInfo, expNode));
        }

        var body = Expression.MemberInit(
            Expression.New(typeof(T)),
            bindings
        );

        return Expression.Lambda<Func<T, object>>(body, model);
    }
}