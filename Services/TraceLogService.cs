using OpenessApp.Models;
using System.Collections.ObjectModel;

namespace OpenessApp.Services
{
    public class TraceLogService
    {
        public ObservableCollection<TraceLogEntry> Entries { get; }

        public TraceLogService()
        {
            Entries = new ObservableCollection<TraceLogEntry>();
        }

        public void Write(string message, string type = "INFO")
        {
            Entries.Add(new TraceLogEntry
            {
                Message = message,
                Type = type
            });
        }
    }
}
