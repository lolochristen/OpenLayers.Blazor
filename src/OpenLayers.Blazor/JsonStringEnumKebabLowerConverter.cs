using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;


#if NET8_0
internal class JsonStringEnumKebabLowerConverter : JsonStringEnumConverter
{
    public JsonStringEnumKebabLowerConverter() : base(JsonNamingPolicy.KebabCaseLower, false)
    {
    }
}
#else

internal class JsonStringEnumKebabLowerConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(JsonStringEnumKebabLowerConverter<>).MakeGenericType(typeToConvert);
        return Activator.CreateInstance(converterType) as JsonConverter;
    }
}

internal class JsonStringEnumKebabLowerConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Enum.Parse<TEnum>(value, true);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        var s = value.ToString();
        for (int i = 1; i < s.Length; i++)
        {
            if (char.IsLower(s[i-1]) && char.IsUpper(s[i]))
            {
                s = s.Insert(i, "-");
                i++;
            }
        }
        writer.WriteStringValue(s.ToLowerInvariant());
    }
}

#endif
