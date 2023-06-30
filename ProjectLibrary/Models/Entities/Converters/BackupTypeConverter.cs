using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectLibrary.Models.Converters
{
    class BackupTypeConverter : JsonConverter<BackupType>
    {
        public override BackupType Read(ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
        {

            if (reader.TokenType == JsonTokenType.String)
            {
                return (BackupType)Enum.Parse(typeof(BackupType), reader.GetString(), true);
            }else if(reader.TokenType == JsonTokenType.Number)
            {
                return (BackupType)reader.GetInt32();
            }
            else
            {
                Console.WriteLine($"Unsupported token type: {reader.TokenType}");

                throw new System.Text.Json.JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, BackupType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
