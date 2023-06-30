using System;

namespace ProjectLibrary.Models
{
    public class Log
    {

        private string name;

        private string fileSource;

        private string fileDestination;

        private long fileSize;

        private double transferTime;

        private double encryptionTime;

        private DateTime startDate;

        public long FileSize { get => fileSize; set => fileSize = value; }
        public double TransferTime { get => transferTime; set => transferTime = value; }
        public DateTime StartDate { get => startDate; set => startDate = value; }
        public string Name { get => name; set => name = value; }
        public string FileSource { get => fileSource; set => fileSource = value; }
        public string FileDestination { get => fileDestination; set => fileDestination = value; }
        public double EncryptionTime { get => encryptionTime; set => encryptionTime = value; }

        public override string ToString()
        {
            return this.FileSize.ToString();
        }
    }
}
