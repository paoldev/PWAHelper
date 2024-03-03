using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms.Design;

namespace PWAHelper
{
    // Additional json naming policies: SpaceCaseLower and SpaceCaseUpper.
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


        //Change the separator char in the given string, used by EnumFlags serializers.
        public static string? ChangeStringSeparator(string? s, char sourceSeparator, char destSeparator) => s?.Trim().
            Split(sourceSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Aggregate((n, x) => n + destSeparator + x) ?? s;

        //Convert a string according to the passed JsonNamingPolicy.
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
                const char s_defaultStringSeparator = ',';
                if (s.Contains(s_defaultStringSeparator))    //Flags?
                {
                    if (!enumType.IsDefined(typeof(FlagsAttribute), false))
                    {
                        throw new ArgumentException(enumType.Name, nameof(enumType));
                    }

                    s = s.Split(s_defaultStringSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).
                        Select(x => GetUnderlyingEnumStringInternal(enumType, namingPolicy, x)).Aggregate((n, x) => n + s_defaultStringSeparator + " " + x);
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

    // Attribute to serialize enums as strings formatted by JsonNamingPolicy.

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

    // Enum converter for json files.
    internal class PWAJsonStringEnumConverter<T> : JsonStringEnumConverter<T> where T : struct, Enum
    {
        public PWAJsonStringEnumConverter() : base(JsonNamingPolicyEnumAttribute.FindJsonNamingPolicy(typeof(T)), JsonNamingPolicyEnumAttribute.FindAllowIntegerValues(typeof(T)))
        {
        }
    }

    // PropertyGrid Enum viewer, to show enums as strings formatted by JsonNamingPolicy.
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

    // Color serializers for json files.
    internal class PWAColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (Color)(new ColorConverter())!.ConvertFrom(reader.GetString()!)!;

        public override void Write(Utf8JsonWriter writer, Color colorValue, JsonSerializerOptions options) => writer.WriteStringValue($"#{colorValue.R:X2}{colorValue.G:X2}{colorValue.B:X2}");
    }

    // PropertyGrid Collections title customization.
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

    // Enum Flags: use spaces in place of commas in PropertyGrid editor.
    internal partial class PWASpacedEnumFlagsEditorConverter(Type type) : PWAEnumEditorConverter(type)
    {
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destType)
        {
            object? result = base.ConvertTo(context, culture, value, destType);

            if (result is string s)
            {
                return MyJsonNamingPolicy.ChangeStringSeparator(s, ',', ' ');
            }

            return result;
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string s)
            {
                value = MyJsonNamingPolicy.ChangeStringSeparator(s, ' ', ',')!;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    // Enum Flags: use spaces in place of commas when (de)serializing json files.
    internal class PWASpacedEnumFlagsJsonConverter<T> : JsonConverter<T> where T : Enum
    {
        private readonly JsonNamingPolicy? _namingPolicy = JsonNamingPolicyEnumAttribute.FindJsonNamingPolicy(typeof(T));

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = MyJsonNamingPolicy.ChangeStringSeparator(reader.GetString(), ' ', ',');
            if (s == null)
            {
                return default;
            }

            return (T)Enum.Parse(typeof(T), MyJsonNamingPolicy.GetUnderlyingEnumString(typeof(T), _namingPolicy, s)!, true);
        }

        public override void Write(Utf8JsonWriter writer, T enumValue, JsonSerializerOptions options) => 
            writer.WriteStringValue(MyJsonNamingPolicy.ChangeStringSeparator(MyJsonNamingPolicy.ConvertString(_namingPolicy, enumValue.ToString()), ',', ' '));
    }

    // Enum Flags: show checkboxes on the left side of enum entries when editing Enum Flags fields in PropertyGrid.
    internal class PWAEnumFlagsEditorUI(Type type) : UITypeEditor
    {
        private class PWACheckedListBoxItem(string item1, long item2) : Tuple<string, long>(item1, item2)
        {
            public override string ToString() => Item1.ToString();
        }

        private readonly Type _enumType = type;
        private readonly JsonNamingPolicy? _namingPolicy = JsonNamingPolicyEnumAttribute.FindJsonNamingPolicy(type);
        private readonly CheckedListBox _checkedListBox = new();

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            if (context?.Instance != null && provider != null)
            {
                if (provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService service)
                {
                    bool isUnderlyingTypeUInt64 = Enum.GetUnderlyingType(_enumType) == typeof(ulong);

                    long flagsValue = isUnderlyingTypeUInt64 ? unchecked((long)Convert.ToUInt64(value)) : Convert.ToInt64(value);

                    _checkedListBox.CheckOnClick = true;
                    _checkedListBox.Items.Clear();
                    foreach (string name in Enum.GetNames(_enumType))
                    {
                        object enumValue = Enum.Parse(_enumType, name);

                        long itemValue = isUnderlyingTypeUInt64 ? unchecked((long)Convert.ToUInt64(enumValue)) : Convert.ToInt64(enumValue);

                        bool bIsChecked = (flagsValue & itemValue) != 0;

                        PWACheckedListBoxItem item = new(MyJsonNamingPolicy.ConvertString(_namingPolicy, name), itemValue);

                        _checkedListBox.Items.Add(item, bIsChecked);
                    }

                    service.DropDownControl(_checkedListBox);

                    long result = 0;
                    foreach (PWACheckedListBoxItem item in _checkedListBox.CheckedItems)
                    {
                        result |= item.Item2;
                    }

                    //At least one flag has to be set; otherwise return the original value.
                    return (result != 0) ? Enum.ToObject(_enumType, result) : value;
                }
            }
            return null;
        }
    }
}
