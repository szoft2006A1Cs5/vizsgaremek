using System.Linq.Expressions;
using System.Reflection;
using backend.Contexts;
using backend.Serialization;

namespace backend.VisibilityFiltering;

public static class FilteredExpressionBuilder
{
    public static Expression<Func<T, object>>? BuildFilteredExpression<T>(Context ctx, int authUser) where T : IFilterable<T>
    {
        var model = Expression.Parameter(typeof(T), "model");
        var bindings = new List<MemberBinding>();

        var modelProps = ctx.Model.FindEntityType(typeof(T));

        if (modelProps == null) return null;

        foreach (var prop in modelProps.GetProperties())
        {
            var propVisibility = prop.PropertyInfo?.GetCustomAttribute<VisibleToAttribute>()?
                                .VisibilityLevel ?? VisibilityLevel.Public;

            if (prop.PropertyInfo == null) continue;

            var valueAssigned = Expression.IfThenElse(
                T.GetVisibilityConditionExpression(propVisibility, authUser),
                Expression.Property(model, prop.PropertyInfo),
                Expression.Constant(null)
            );

            bindings.Add(Expression.Bind(prop.PropertyInfo, valueAssigned));
        }

        var body = Expression.MemberInit(
            Expression.New(typeof(T)),
            bindings
        );

        return Expression.Lambda<Func<T, object>>(body, model);
    }
}