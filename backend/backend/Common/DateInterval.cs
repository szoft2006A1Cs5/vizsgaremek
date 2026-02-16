namespace backend.Common
{
    public class DateInterval
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public DateInterval(DateTime start, DateTime end)
        {
            this.Start = start;
            this.End = end;
        }

        public static bool DoesCollide(this DateInterval lhs, DateInterval rhs)
        {
            return false;
        }
    }
}
