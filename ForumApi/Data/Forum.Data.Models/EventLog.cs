using System;

namespace Forum.Data.Models
{
    public class EventLog : BaseModel<int>
    {
        public int EventId { get; set; }

        public string LogLevel { get; set; }

        public string Message { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
