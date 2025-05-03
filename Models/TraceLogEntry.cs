using System;

namespace OpenessApp.Models
{
    public class TraceLogEntry
    {
        public string Message { get; set; }
        public string Type { get; set; } // "INFO", "ERROR", "WARN"

        public string Timestamp => DateTime.Now.ToString("HH:mm:ss");

        public string FullMessage => $"[{Timestamp}] {Message}";
    }
}
