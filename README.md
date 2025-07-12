# Scheduler_BackEnd

To run a program you need:
 - Copy repository to your local machine;
 - Open file Scheduler_BackEnd.sln in VisualStudio ("\Scheduler_Task_BackEnd\Scheduler_BackEnd\Scheduler_BackEnd.sln");
 - Run project using https.

After project running you will see Swagger interface in browser. Here you will have all required endpoints + additional endpoint to add meeting.This endpoint don't have validation of meeting time, it will be better if you firstly check matching slot from /meetings endpoint and use this result in adding meeting endpoint.

**Some limitations:**
- If you use time interval in /meetings endpoint requirements outside working hours, no slot will be provided;

**Test data:**
*- /users (enter one by one)*
 
 {
    "id": 1,
    "name": "Alice"
  }

 {
    "id": 2,
    "name": "Mark"
  }

 {
    "id": 3,
    "name": "Michael"
  }
  
*- /meetings/add*
 
 {
    "id": 1,
    "participants": [ 1,2 ],
    "startTime": "2025-07-12T12:00:00.026Z",
    "endTime": "2025-07-12T12:30:00.026Z"
  }

 {
    "id": 2,
    "participants": [ 2,3 ],
    "startTime": "2025-07-12T14:00:00.713Z",
    "endTime": "2025-07-12T15:00:00.713Z"
  }

*- /meetings*
 
 {
    "participantsIds": [1, 2, 3],
    "durationMinutes": 60,
    "earliestStart": "2025-07-12T09:00:00Z",
    "latestEnd": "2025-07-12T17:00:00Z"
  }

 {
    "participantsIds": [1, 2, 3],
    "durationMinutes": 80,
    "earliestStart": "2025-07-12T12:00:00Z",
    "latestEnd": "2025-07-12T16:30:00Z"
  }

 {
    "participantsIds": [1, 2],
    "durationMinutes": 90,
    "earliestStart": "2025-07-12T12:00:00Z",
    "latestEnd": "2025-07-12T17:00:00Z"
  }

 {
    "participantsIds": [1, 2, 3],
    "durationMinutes": 90,
    "earliestStart": "2025-07-12T12:00:00Z",
    "latestEnd": "2025-07-12T15:00:00Z"
  }
