using Scheduler_BackEnd.Models;

namespace Scheduler_BackEnd
{
    public interface IScheduleService
    {
        public TimeSlot? FindMatchingTimeSlot(MeetingRequirements requirements);
    }
}
