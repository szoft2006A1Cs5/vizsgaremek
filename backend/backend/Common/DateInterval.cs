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
        
        public bool DoesCollide(DateInterval rhs)
        {
            return !((this.End < rhs.Start && this.End < rhs.End) ||
                     (this.Start < rhs.Start && this.End < rhs.Start) ||
                     (rhs.End < this.Start && rhs.End < this.End) ||
                     (rhs.Start < this.Start && rhs.End < this.Start));
        }
        
        public bool DoesContain(DateInterval rhs, bool inclusive = true)
        {
            return (inclusive ? 
                this.Start <= rhs.Start && rhs.End <= this.End : 
                this.Start < rhs.Start && rhs.End < this.End);
        }
    }
}
