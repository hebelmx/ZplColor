using Newtonsoft.Json;
using System.Drawing;
using ZplColor.Common;

namespace PrinterTester;

public class ColorConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(RgbColor));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var s = (string)reader.Value;
        var parts = s.Split(',');
        return Color.FromArgb(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()), int.Parse(parts[2].Trim()));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var c = (RgbColor)value;
        var colorString = $"{c.Red}, {c.Green}, {c.Blue}";
        writer.WriteValue(colorString);
    }
}