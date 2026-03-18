using ZstdSharp.Unsafe;

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
        
        public bool DoesCollide(DateInterval rhs, bool inclusive = true)
        {
            return inclusive ? 
                !(this.End <= rhs.Start || rhs.End <= this.Start) : 
                !(this.End < rhs.Start || rhs.End < this.Start);
        }
    }
}
