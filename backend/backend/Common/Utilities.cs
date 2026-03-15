using System.Linq.Expressions;
using backend.Models;

namespace backend.Common
{
    public static class Utilities
    {
        public static int MaxOrZero<T>(this IEnumerable<T> enu, Func<T, int?> selector) where T : class
        {
            return enu.Max(selector) ?? 0;
        }
        
        public static int MaxOrZero<T>(this IQueryable<T> que, Expression<Func<T, int?>> selector) where T : class
        {
            return que.Max(selector) ?? 0;
        }
    }
}
