namespace Scheduler_BackEnd.Models
{
    public class TimeSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public double Duration {
            get
            {
                return (EndTime - StartTime).TotalMinutes;
            }
         }
    }
}
