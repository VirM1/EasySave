using System.Text.Json.Serialization;
using ProjectLibrary.Models.Converters;

namespace ProjectLibrary.Models
{
    public class Work
    {
        private Backup backup;//label, source, destination, mode

        protected WorkStatus status;

        protected int initialFileCount;

        protected int remainingFileCount;

        protected long initialFileSize;

        protected long remainingFileSize;

        protected string currentSourcePath;

        protected string currentDestinationPath;

        protected bool finishedPriority;

        private int position;

        public Backup Backup { get => backup; set => backup = value; }
        public int InitialFileCount { get => initialFileCount; set => initialFileCount = value; }
        public long InitialFileSize { get => initialFileSize; set => initialFileSize = value; }
        public int RemainingFileCount { 
            get => remainingFileCount;
            set
            {
                remainingFileCount = value;
                if(remainingFileCount == 0)
                {
                    Status = WorkStatus.End;
                }
            }
        }
        public long RemainingFileSize { get => remainingFileSize; set => remainingFileSize = value; }

        [JsonConverter(typeof(WorkStatusConverter))]
        public WorkStatus Status { get => status; set => status = value; }
        public string CurrentSourcePath { get => currentSourcePath; set => currentSourcePath = value; }
        public string CurrentDestinationPath { get => currentDestinationPath; set => currentDestinationPath = value; }

        [JsonIgnore]
        public int Position { get => position; set => position = value; }

        [JsonIgnore]
        public bool FinishedPriority { get => finishedPriority; set => finishedPriority = value; }
    }
}
