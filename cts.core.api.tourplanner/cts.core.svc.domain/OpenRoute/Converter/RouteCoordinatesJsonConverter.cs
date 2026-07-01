using System.Text.Json;
using System.Text.Json.Serialization;

namespace cts.core.svc.domain.OpenRoute.Converter;

public class RouteCoordinatesJsonConverter : JsonConverter<RouteCoordinates>
{
    public override RouteCoordinates Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        reader.Read(); // StartArray

        double longitude = reader.GetDouble();

        reader.Read();

        double latitude = reader.GetDouble();

        reader.Read(); // EndArray

        return new RouteCoordinates(longitude, latitude);
    }

    public override void Write(
        Utf8JsonWriter writer,
        RouteCoordinates value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Longitude);
        writer.WriteNumberValue(value.Latitude);
        writer.WriteEndArray();
    }
}