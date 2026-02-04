using System.Linq.Expressions;
using System.Reflection;
using backend.Contexts;
using backend.Serialization;
using Microsoft.EntityFrameworkCore;

namespace backend.VisibilityFiltering;

public static class FilteredExpressionBuilder
{
    public static IQueryable<object>? FilterVisibility<T>(this IQueryable<T> model, DbContext context, int? authUser) where T : class, IFilterable<T>
    {
        var filteringExp = BuildFilteredExpression<T>(context, authUser);
        return filteringExp != null ? model.Select(filteringExp) : null;
    }

    public static Expression<Func<T, object>>? BuildFilteredExpression<T>(DbContext ctx, int? authUser) where T : class, IFilterable<T>
    {
        var param = Expression.Parameter(typeof(T), "model");
        var bindings = new List<MemberBinding>();

        var modelProps = ctx.Model.FindEntityType(typeof(T));

        if (modelProps == null) return null;

        foreach (var prop in modelProps.GetProperties())
        {
            var propVisibility = prop.PropertyInfo?.GetCustomAttribute<VisibleToAttribute>()?
                                .VisibilityLevel ?? VisibilityLevel.Public;

            if (prop.PropertyInfo == null) continue;

            var valueAssigned = Expression.IfThenElse(
                Expression.Invoke(T.GetVisibilityConditionExpression(propVisibility, authUser), param),
                Expression.Property(param, prop.PropertyInfo),
                Expression.Default(prop.PropertyInfo.PropertyType)
            );

            bindings.Add(Expression.Bind(prop.PropertyInfo, valueAssigned));
        }

        /*
         Ezt kellene legeneralnia, Expressionbol C# eseten:
          x => new {
              ...
              Prop = <Condition>(x) ? x.Prop : default,
              ...
          }
         
         Ezt pedig Expressionbol SQL eseten:
          SELECT ..., CASE <Condition> THEN Prop ELSE NULL END, ... FROM model;
        */
        var body = Expression.MemberInit(
            Expression.New(typeof(T)),
            bindings
        );

        return Expression.Lambda<Func<T, object>>(body, param);
    }
}