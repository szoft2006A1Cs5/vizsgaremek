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
    private static Dictionary<Type, Expression?> expressionCache = new();
    
    public static IQueryable<object>? FilterVisibility<T>(this IQueryable<T> model, DbContext context, User? authUser) where T : class
    {
        if (!expressionCache.TryGetValue(typeof(T), out var expression) || expression == null)
        {
            expression = BuildFilteredExpression<T>(context, authUser, []);
            expressionCache.Add(typeof(T), expression);
        }

        var filteringExp = expression as Expression<Func<T, object>>;
        
        return filteringExp != null ? model.Select(filteringExp) : null;
    }

    private static Expression<Func<T, object>>? BuildFilteredExpression<T>(DbContext ctx, User? authUser, Type[] exploredTypes) where T : class
    {
        if (exploredTypes.Contains(typeof(T))) return null;
        
        var param = Expression.Parameter(typeof(T), "model");
        var bindings = new List<MemberBinding>();

        var modelProps = ctx.Model.FindEntityType(typeof(T));

        if (modelProps == null) return null;
        
        foreach (var prop in modelProps.GetProperties().Concat<IPropertyBase>(modelProps.GetNavigations()))
        {
            var propVisibility = prop.PropertyInfo?.GetCustomAttribute<VisibleToAttribute>()?
                                .VisibilityLevel ?? VisibilityLevel.Public;

            if (prop.PropertyInfo == null) continue;

            Expression assignmentExpression;

            var filterable = typeof(T)
                    .GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IFilterable<>));

            if (filterable != null)
            {
                // Az IFilterable lete elmeletileg garantalja, hogy a metodus ilyen formaban
                // (return value, parameterek) letezik.
                var getVisConExpMethod = typeof(T).GetMethod("GetVisibilityConditionExpression");
                if (getVisConExpMethod == null) continue;

                var visibilityConditionExpression = getVisConExpMethod.Invoke(null, [propVisibility, authUser]) as Expression;
                if (visibilityConditionExpression == null) continue; // Hat ha ez osszejonne itt valami nagyon rossz.

                MemberExpression valueAssigned;

                if (prop is INavigation navProp)
                {
                    /*
                    var buildFilteredExp = typeof(FilteredExpressionBuilder).GetMethod("BuildFilteredExpression");
                    if (buildFilteredExp == null) continue; // Ez igy amugy tulajdonkeppen lehetetlen kell hogy legyen

                    // Megkene oldani, hogy ne legyen vegtelen rekurzio (IQueryable ExpTree include scanning? (U.i.: Miert utalom magam?))
                    // (Vagy csak szimplan a mar beincludeolt typeokat nem includeolja be tobbet, mondjuk?)
                    var buildFilteredExpForType = buildFilteredExp.MakeGenericMethod(navProp.TargetEntityType.ClrType);
                    var filteredExp = buildFilteredExpForType.Invoke(null, [ctx, authUser, exploredTypes.Append(typeof(T))]) as Expression;
                    if (filteredExp == null) continue;
                    */
                    if (navProp.IsCollection)
                    {
                        valueAssigned = Expression.Property(param, prop.PropertyInfo);
                    }
                    else
                    {
                        valueAssigned = Expression.Property(param, prop.PropertyInfo);
                    }
                } else
                {
                    valueAssigned = Expression.Property(param, prop.PropertyInfo);
                }

                assignmentExpression = Expression.Condition(
                    Expression.Invoke(visibilityConditionExpression, param),
                    valueAssigned,
                    Expression.Default(prop.PropertyInfo.PropertyType)
                );
            }
            else
            {
                assignmentExpression = Expression.Property(param, prop.PropertyInfo);
            }

            // TODO: Tok mindegy mit csinalok ez itt igy nem lesz jo, mert habar ez a type
            //       nem IFilterable, lehet egy benne nestelt az. Ugyhogy minden nav propnak vegig kell
            //       majd mennie a filterelesen. Ez eddig igy nem tul szep. Esetleges megoldas, hogy valahogy
            //       az IQueryable Expression Tree-jeben kutyulok ossze valamit, hogy a filtereles megtortenjen,
            //       de abba meg nem nagyon neztem bele, ez inkabb csak egy otlet.

            // TODO: Hogyan tartom fent az include nestelest, ha itt egy uj select jon letre a filtereles miatt?
            //       Kovetkezo kerdes, egyaltalan peldaul hogyan selectelek egy IEnumerablebe?

            bindings.Add(Expression.Bind(prop.PropertyInfo, assignmentExpression));
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