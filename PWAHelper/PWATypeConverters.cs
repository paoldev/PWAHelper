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


        //Convert a string according to the passed JsonNamingPolicy
        public static string ConvertString(JsonNamingPolicy? namingPolicy, string s) => namingPolicy?.ConvertName(s) ?? s;


        //Converts a string already converted by JsonNamingPolicy into a new string using the original enum names.
        public static string? GetUnderlyingEnumString(Type enumType, JsonNamingPolicy? namingPolicy, string s)
        {
            if (s == null)
            {
                return null;
            }

            if (namingPolicy != null)
            {
                if (s.Contains(','))    //Flags?
                {
                    if (!enumType.IsDefined(typeof(FlagsAttribute), false))
                    {
                        throw new ArgumentException(enumType.Name, nameof(enumType));
                    }

                    s = s.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).
                        Select(x => GetUnderlyingEnumStringInternal(enumType, namingPolicy, x)).Aggregate((s, x) => (s + ", " + x));
                }
                else
                {
                    s = GetUnderlyingEnumStringInternal(enumType, namingPolicy, s);
                }
            }
            return s;
        }

        private static string GetUnderlyingEnumStringInternal(Type enumType, JsonNamingPolicy namingPolicy, string s)
        {
            string[] names = Enum.GetNames(enumType);
            foreach (var name in names)
            {
                if (namingPolicy.ConvertName(name) == s)
                {
                    return name;
                }
            }
            return s;
        }
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

    internal class PWAEnumEditorConverter(Type type) : EnumConverter(type)
    {
        private readonly Type _enumType = type;
        private readonly JsonNamingPolicy? _namingPolicy = JsonNamingPolicyEnumAttribute.FindJsonNamingPolicy(type);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destType)
        {
            var result = base.ConvertTo(context, culture, value, destType);
            if (result is string s)
            {
                result = MyJsonNamingPolicy.ConvertString(_namingPolicy, s);
            }
            return result;
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if ((_namingPolicy != null) && (value is string s))
            {
                value = MyJsonNamingPolicy.GetUnderlyingEnumString(_enumType, _namingPolicy, s)!;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    internal class PWAColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (Color)(new ColorConverter())!.ConvertFrom(reader.GetString()!)!;

        public override void Write(Utf8JsonWriter writer, Color colorValue, JsonSerializerOptions options) => writer.WriteStringValue($"#{colorValue.R:X2}{colorValue.G:X2}{colorValue.B:X2}");
    }

    internal class PWAExpandableObjectEditorConverter : TypeConverter // ExpandableObjectConverter
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
