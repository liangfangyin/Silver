using System;

namespace  Silver.WatchDog.src.Models
{
    public class WatchLoggerModel
    {
        public int Id { get; set; }
        public string EventId { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string CallingFrom { get; set; } = "";
        public string CallingMethod { get; set; } = "";
        public int LineNumber { get; set; }
        public string LogLevel { get; set; } = "";
    }
}
