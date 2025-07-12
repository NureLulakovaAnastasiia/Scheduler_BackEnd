using Scheduler_BackEnd.Controllers;
using Scheduler_BackEnd.Models;

namespace Scheduler_BackEnd
{
    public class ScheduleService : IScheduleService
    {
        private int workStartHour = 9;
        private int workEndHour = 17;
        private MeetingRequirements meetingRequirements;


        public TimeSlot? FindMatchingTimeSlot(MeetingRequirements requirements)
        {
            meetingRequirements = requirements;

            if (meetingRequirements.DurationMinutes == 0 
                || !IfWorkTime() 
                || !IfParticipantsExist())
            {
                return null;
            }

            var freeParticipantsSlots = new Dictionary<int, List<TimeSlot>>();

            foreach (var partId in requirements.ParticipantsIds)
            {
                freeParticipantsSlots.Add(partId, GetFreeParticipantSlots(partId));
            }

            //find timeslots which are free for everyone
            var allMatchingSlots = GetMatchingTimeSlots(freeParticipantsSlots);

            return allMatchingSlots.MinBy(s => s.StartTime);
        }

        private bool IfWorkTime()
        {
            var startHour = meetingRequirements.EarliestStart.ToUniversalTime().Hour;
            var endHour = meetingRequirements.LatestEnd.ToUniversalTime().Hour;

            return (startHour >= workStartHour && startHour < workEndHour)
                && (endHour > workStartHour && endHour <= workEndHour);
        }
        private bool IfParticipantsExist()
        {
            var existingMatchingUsers = meetingRequirements.ParticipantsIds
                .Except(DataStorage.Users
                    .Select(u => u.Id));
                
             return existingMatchingUsers.Count() == 0;
                
        }
        private List<TimeSlot> GetFreeParticipantSlots(int participantId)
        {
            var result = new List<TimeSlot>();

            var existingMeetings = DataStorage.Meetings
                .Where(m => m.Participants.Contains(participantId)
                    && m.StartTime.Date == meetingRequirements.EarliestStart.Date)
                .OrderBy(m => m.StartTime)
                .ToList();

            //creates timeslots from free time between existing meetings
            if (existingMeetings.Count > 0)
            {
                var startTime = meetingRequirements.EarliestStart.Date + new TimeSpan(workStartHour, 0, 0); 
                
                foreach (var meeting in existingMeetings)
                {
                    if(startTime >= meetingRequirements.LatestEnd)
                    {
                        break;
                    }
                    var difference = (meeting.StartTime - startTime).TotalMinutes;
                    if(difference >= meetingRequirements.DurationMinutes)
                    {
                        result.Add(new TimeSlot
                        {
                            StartTime = startTime,
                            EndTime = meeting.StartTime
                        });
                    }
                    startTime = meeting.EndTime.AddMinutes(1);
                }
                //adds free slot from between last meeting and latest required time
                if(meetingRequirements.LatestEnd > startTime
                    && (meetingRequirements.LatestEnd - startTime).TotalMinutes >= meetingRequirements.DurationMinutes)
                {
                    result.Add(new TimeSlot
                    {
                        StartTime = startTime,
                        EndTime = meetingRequirements.LatestEnd
                    });
                }
            }
            else
            {
                result.Add(new TimeSlot()
                {
                    StartTime = meetingRequirements.EarliestStart,
                    EndTime = meetingRequirements.LatestEnd
                });
            }

            return result;
        }

        private List<TimeSlot> GetMatchingTimeSlots(Dictionary<int, List<TimeSlot>> data)
        {
            if (data.Count == 1)
            {
                return data[0];
            }

            var result = new List<TimeSlot>();

            if (data.Count > 1)
            {

                var busiest = 0;
                var longest = 0;
                foreach (var item in data)
                {
                    if(item.Value.Count > longest)
                    {
                        longest = item.Value.Count;
                        busiest = item.Key;
                    }
                }

                //use first user's slots to find overlapping slots from other users
                for (int i = 0; i < data[busiest].Count; i++)
                {
                    var timeSlot = data[busiest][i];
                    if(timeSlot.Duration < meetingRequirements.DurationMinutes
                        || timeSlot.EndTime < meetingRequirements.EarliestStart)
                    {
                        continue;
                    }

                    //fit slot start time to requirements start time if needed
                    if(timeSlot.StartTime < meetingRequirements.EarliestStart 
                        && meetingRequirements.EarliestStart < timeSlot.EndTime)
                    {
                        timeSlot.StartTime = meetingRequirements.EarliestStart;
                        if(timeSlot.Duration < meetingRequirements.DurationMinutes)
                        {
                            continue;
                        }
                    }

                    var numOfOverlapping = 1;
                    var overlappedSlots = new List<TimeSlot>() { timeSlot};

                    //checks other users' slots for matching
                    for (int j = 0; j < data.Count; j++)
                    {
                        if(data.Keys.ElementAt(j) == busiest)
                        {
                            continue;
                        }
                        var temp = data[data.Keys.ElementAt(j)]
                            .FindAll(s => s.StartTime <= timeSlot.EndTime
                            && timeSlot.StartTime <= s.EndTime
                            && s.Duration >= meetingRequirements.DurationMinutes)
                            .MinBy(s => s.StartTime);
                            
                        
                        if (temp != null)
                        {
                            numOfOverlapping++;
                            overlappedSlots.Add(temp);
                        }
                        else
                        {
                            //no overlapped slots from user - go to next first user's slot
                            break;
                        }
                    }

                    //true only when every required participant has matching free slot
                    if (numOfOverlapping == data.Count)
                    {
                        var slotToAdd = new TimeSlot
                        {
                            StartTime = overlappedSlots.Select(s => s.StartTime).Max(),
                            EndTime = overlappedSlots.Select(s => s.EndTime).Min()
                        };
                        if(slotToAdd.Duration >= meetingRequirements.DurationMinutes)
                        {
                            result.Add(slotToAdd);
                        }
                    }

                    numOfOverlapping = 1;
                    overlappedSlots.Clear();
                }
            }
            return result;
        }
    }
}
