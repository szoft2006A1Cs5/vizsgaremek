using System.Linq.Expressions;
using backend.Models;

namespace backend.VisibilityFiltering;

public interface IFilterable
{
    static abstract Func<object?, User?, bool> GetVisibilityConditionLambda(VisibilityLevel visLevel);
}