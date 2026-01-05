using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Infraestructure.Serialization
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {

        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                DateTime dateTime = DateTime.Parse(reader.GetString()!, null, DateTimeStyles.RoundtripKind);
                return DateOnly.FromDateTime(dateTime);
            }
            catch (FormatException)
            {
                return DateOnly.ParseExact(reader.GetString()!, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
        }
    }
}
