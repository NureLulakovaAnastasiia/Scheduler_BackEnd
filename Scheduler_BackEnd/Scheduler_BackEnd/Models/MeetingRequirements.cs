namespace Scheduler_BackEnd.Models
{
    public class MeetingRequirements
    {
        public int[] ParticipantsIds { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime EarliestStart { get; set; }
        public DateTime LatestEnd { get; set; }
    }
}
