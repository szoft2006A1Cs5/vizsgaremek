using backend.Models;

namespace backend.Common
{
    public static class Utilities
    {
        public static int MaxOrZero<T>(this IEnumerable<T> enu, Func<T, int?> selector) where T : class
        {
            return enu.Max(selector) ?? 0;
        }
    }
}
