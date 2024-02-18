using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PWAHelper
{
    internal class MyJsonNamingPolicy : JsonNamingPolicy
    {
        private readonly bool _lowercase;
        private MyJsonNamingPolicy(bool lowercase) { _lowercase = lowercase; }
        public override string ConvertName(string name)
        {
            return _lowercase ?
                SnakeCaseLower.ConvertName(name).Replace('_', ' ') :
                SnakeCaseUpper.ConvertName(name).Replace('_', ' ');
        }

        public static JsonNamingPolicy SpaceCaseLower { get; } = new MyJsonNamingPolicy(true);
        public static JsonNamingPolicy SpaceCaseUpper { get; } = new MyJsonNamingPolicy(false);
    }


    [AttributeUsage(AttributeTargets.Enum)]
    internal class JsonNamingPolicyEnumAttribute : Attribute
    {
        public string? Policy { get; set; }
        public bool AllowIntegerValues { get; set; } = true;

        public static JsonNamingPolicy? FindJsonNamingPolicy(Type type)
        {
            var customAttributes = (JsonNamingPolicyEnumAttribute[])type.GetCustomAttributes(typeof(JsonNamingPolicyEnumAttribute), true);
            if (customAttributes.Length > 0)
            {
                var attribute = customAttributes[0];
                return (attribute.Policy?.ToLower()) switch
                {
                    "camelcase" => JsonNamingPolicy.CamelCase,
                    "snakecaselower" => JsonNamingPolicy.SnakeCaseLower,
                    "snakecaseupper" => JsonNamingPolicy.SnakeCaseUpper,
                    "kebabcaselower" => JsonNamingPolicy.KebabCaseLower,
                    "kebabcaseupper" => JsonNamingPolicy.KebabCaseUpper,
                    "spacecaselower" => MyJsonNamingPolicy.SpaceCaseLower,
                    "spacecaseupper" => MyJsonNamingPolicy.SpaceCaseUpper,
                    _ => throw new ArgumentException($"Invalid attribute policy {attribute.Policy}"),
                };
            }
            return null;
        }

        public static bool FindAllowIntegerValues(Type type)
        {
            const bool DEFAULT_VALUE = true;    //default value for JsonStringEnumConverter<T>
            var customAttributes = (JsonNamingPolicyEnumAttribute[])type.GetCustomAttributes(typeof(JsonNamingPolicyEnumAttribute), true);
            if (customAttributes.Length > 0)
            {
                var attribute = customAttributes[0];
                return attribute.AllowIntegerValues;
            }
            return DEFAULT_VALUE;
        }
    }

    internal class PWAJsonStringEnumConverter<T> : JsonStringEnumConverter<T> where T : struct, Enum
    {
        public PWAJsonStringEnumConverter() : base(JsonNamingPolicyEnumAttribute.FindJsonNamingPolicy(typeof(T)), JsonNamingPolicyEnumAttribute.FindAllowIntegerValues(typeof(T)))
        {
        }
    }

    internal class PWAEnumConverter(Type type) : EnumConverter(type)
    {
        private readonly Type enumType = type;
        private readonly JsonNamingPolicy? namingPolicy = JsonNamingPolicyEnumAttribute.FindJsonNamingPolicy(type);

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destType)
        {
            return destType == typeof(string);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destType)
        {
            if (value != null)
            {
                string? name = Enum.GetName(enumType, value);
                if (name != null)
                {
                    return namingPolicy?.ConvertName(name) ?? name;
                }
            }
            return base.ConvertTo(context, culture, value, destType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (namingPolicy != null)
            {
                string[] names = Enum.GetNames(enumType);
                foreach (var name in names)
                {
                    if (namingPolicy.ConvertName(name) == (string)value)
                    {
                        return Enum.Parse(enumType, name);
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    internal class PWAColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (Color)(new ColorConverter())!.ConvertFrom(reader.GetString()!)!;

        public override void Write(Utf8JsonWriter writer, Color colorValue, JsonSerializerOptions options) => writer.WriteStringValue($"#{colorValue.R:X2}{colorValue.G:X2}{colorValue.B:X2}");
    }

    internal class PWAExpandableObjectConverter : TypeConverter // ExpandableObjectConverter
    {
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            ArgumentNullException.ThrowIfNull(destinationType);

            if (destinationType == typeof(string))
            {
                if (value is System.Collections.ICollection collection)
                {
                    if (collection.Count == 0)
                    {
                        return "(Empty collection)";
                    }
                    else if (collection.Count == 1)
                    {
                        return "(1 element)";
                    }

                    return $"({collection.Count} elements)";
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
