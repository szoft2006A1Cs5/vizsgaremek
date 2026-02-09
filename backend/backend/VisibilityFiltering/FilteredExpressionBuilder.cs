using System.Linq.Expressions;
using System.Reflection;
using backend.Contexts;
using backend.Models;
using backend.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace backend.VisibilityFiltering;

public static class FilteredExpressionBuilder
{
    // TODO: Ez most konkretan csak az IFilterable-os osztalyokat szuri meg,
    //       nekunk meg kellene a lehetoseg, hogy ICollectionokre, sot meg
    //       nem IFilterable-os osztalyokra is rarakhassuk a FilterVisibility-t,
    //       hogy amennyiben azoknak lenne IFilterable-os gyermekobjektumuk/kollekciojuk,
    //       akkor azok filterolodjenek.
    //       + nyilvan a nestelodes (sidenote: asszem ezzel kinyirjuk majd a .Include()-okat).
    
    public static IQueryable<object>? FilterVisibility<T>(this IQueryable<T> model, DbContext context, User? authUser) where T : class, IFilterable<T>
    {
        var filteringExp = BuildFilteredExpression<T>(context, authUser);
        return filteringExp != null ? model.Select(filteringExp) : null;
    }

    private static Expression<Func<T, object>>? BuildFilteredExpression<T>(DbContext ctx, User? authUser) where T : class, IFilterable<T>
    {
        var param = Expression.Parameter(typeof(T), "model");
        var bindings = new List<MemberBinding>();

        var modelProps = ctx.Model.FindEntityType(typeof(T));

        if (modelProps == null) return null;
        
        foreach (var prop in modelProps.GetProperties().Concat<IPropertyBase>(modelProps.GetNavigations()))
        {
            var propVisibility = prop.PropertyInfo?.GetCustomAttribute<VisibleToAttribute>()?
                                .VisibilityLevel ?? VisibilityLevel.Public;

            if (prop.PropertyInfo == null) continue;

            Expression valueAssigned;

            // TODO: Tok mindegy mit csinalok ez itt igy nem lesz jo, mert habar ez a type
            //       nem IFilterable, lehet egy benne nestelt az. Ugyhogy minden nav propnak vegig kell
            //       majd mennie a filterelesen. Ez eddig igy nem tul szep. Esetleges megoldas, hogy valahogy
            //       az IQueryable Expression Tree-jeben kutyulok ossze valamit, hogy a filtereles megtortenjen,
            //       de abba meg nem nagyon neztem bele, ez inkabb csak egy otlet.
            if (prop is INavigation navProp &&
                navProp.TargetEntityType.ClrType
                    .GetInterfaces()
                    .Any(x => x.IsGenericType &&
                              x.GetGenericTypeDefinition() == typeof(IFilterable<>))
                )
            {
                // TODO: Hogyan tartom fent az include nestelest, ha itt egy uj select jon letre a filtereles miatt?
                //       Kovetkezo kerdes, egyaltalan peldaul hogyan selectelek egy IEnumerablebe?
                valueAssigned = Expression.Default(prop.PropertyInfo.PropertyType);
            }
            else
            {
                valueAssigned = Expression.Condition(
                    Expression.Invoke(T.GetVisibilityConditionExpression(propVisibility, authUser), param),
                    Expression.Property(param, prop.PropertyInfo),
                    Expression.Default(prop.PropertyInfo.PropertyType)
                );
            }

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