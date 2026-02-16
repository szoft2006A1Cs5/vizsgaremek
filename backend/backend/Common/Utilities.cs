namespace backend.Common
{
    public static class Utilities
    {
        public static bool DoesCollide(this DateInterval lhs, DateInterval rhs)
        {
            return !((lhs.End < rhs.Start && lhs.End < rhs.End) ||
                     (lhs.Start < rhs.Start && lhs.End < rhs.Start) ||
                     (rhs.End < lhs.Start && rhs.End < lhs.End) ||
                     (rhs.Start < lhs.Start && rhs.End < lhs.Start));
        }
    }
}
