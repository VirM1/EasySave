using System.Collections.Generic;

namespace ProjectLibrary.Models
{
    public class LogList
    {
        private List<Log> logs;
        public List<Log> Logs { get => logs; set => logs = value; }

        public LogList()
        {
            this.logs = new List<Log>();
        }

        public List<Log> GetLogs()
        {
            return this.logs;
        }

        public void AddLog(Log log)
        {
            this.logs.Add(log);
        }
    }
}
