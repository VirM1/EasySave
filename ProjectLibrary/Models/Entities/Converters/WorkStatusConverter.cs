using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectLibrary.Models.Converters
{
    public class WorkStatusConverter : JsonConverter<WorkStatus>
    {
        public override WorkStatus Read(ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
        {

            if (reader.TokenType == JsonTokenType.String)
            {
                return (WorkStatus)Enum.Parse(typeof(WorkStatus), reader.GetString(), true);
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return (WorkStatus)reader.GetInt32();
            }
            else
            {
                Console.WriteLine($"Unsupported token type: {reader.TokenType}");

                throw new System.Text.Json.JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, WorkStatus value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
