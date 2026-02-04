using System.Linq.Expressions;
using backend.Serialization;

namespace backend.VisibilityFiltering;

public interface IFilterable<T> where T : IFilterable<T>
{
    static abstract Expression<Func<T, bool>> GetVisibilityConditionExpression(VisibilityLevel visLevel, int? authUserId);
}