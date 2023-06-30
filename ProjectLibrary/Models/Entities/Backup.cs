using System.Text.Json.Serialization;
using ProjectLibrary.Models.Converters;

namespace ProjectLibrary.Models
{
    public class Backup
    {

        private string label;

        private BackupType type;

        private string source;

        private string destination;

        public string Label { get => label; set => label = value; }
        public string Source { get => source; set => source = value; }
        public string Destination { get => destination; set => destination = value; }

        [JsonConverter(typeof(BackupTypeConverter))]
        public BackupType Type { get => type; set => type = value; }

        public override bool Equals(object obj)
        {
            var other = obj as Backup;
            if (other == null)
            {
                return false;
            }
            return Label == other.Label;
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }
    }
}
