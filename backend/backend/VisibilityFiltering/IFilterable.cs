using System.Linq.Expressions;
using backend.Serialization;
using backend.Models;

namespace backend.VisibilityFiltering;

public interface IFilterable<T> where T : class, IFilterable<T>
{
    static abstract Expression<Func<T?, User?, bool>> GetVisibilityConditionExpression(VisibilityLevel visLevel);
}