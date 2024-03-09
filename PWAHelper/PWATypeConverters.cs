using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms.Design;
using static PWAHelper.PWADisplayNamePropertyTypeConverterParamsAttribute;

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

        public static JsonNamingPolicy? FindJsonNamingPolicy(string? policy)
        {
            if (policy == null)
            {
                return null;
            }

            return (policy.ToLower()) switch
            {
                "camelcase" => CamelCase,
                "snakecaselower" => SnakeCaseLower,
                "snakecaseupper" => SnakeCaseUpper,
                "kebabcaselower" => KebabCaseLower,
                "kebabcaseupper" => KebabCaseUpper,
                "spacecaselower" => SpaceCaseLower,
                "spacecaseupper" => SpaceCaseUpper,
                _ => throw new ArgumentException($"Invalid policy {policy}"),
            };
        }


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
                return MyJsonNamingPolicy.FindJsonNamingPolicy(attribute.Policy);
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

    // Attribute to make PropertyGrid Enum text-label read-only.
    // Don't specify the attribute in order to use the default PropertyGrid Enum behaviour.
    [AttributeUsage(AttributeTargets.Enum)]
    internal class PWAEnumEditorTextEditableAttribute : Attribute
    {
        public bool ReadOnly { get; set; } = false;

        public static bool? IsReadOnly(Type type)
        {
            var customAttributes = (PWAEnumEditorTextEditableAttribute[])type.GetCustomAttributes(typeof(PWAEnumEditorTextEditableAttribute), true);
            if (customAttributes.Length > 0)
            {
                var attribute = customAttributes[0];
                return attribute.ReadOnly;
            }
            return null;
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

        // Make enum label editable in PropertyGrid.
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => PWAEnumEditorTextEditableAttribute.IsReadOnly(_enumType) ?? base.GetStandardValuesExclusive(context);
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

    // PropertyLabels DisplayNameAttribute, based on JsonNamingPolicy.
    // displayNameSource: if DisplayNameSource.Name, JsonNamingPolicy is applied to property.Name, otherwise it's applied to property.DisplayName.
    // displayNameOverride: if DisplayNameOverride.Yes, JsonNamingPolicy is also applied to properties already declared with [DisplayName] attribute; otherwise the original [DisplayName] attribute is preserved.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class PWADisplayNamePropertyTypeConverterParamsAttribute(string jsonNamingPolicy, DisplayNameSource displayNameSource = DisplayNameSource.Name, DisplayNameOverride displayNameOverride = DisplayNameOverride.No) : Attribute
    {
        private readonly JsonNamingPolicy? _namingPolicy = MyJsonNamingPolicy.FindJsonNamingPolicy(jsonNamingPolicy);
        private readonly DisplayNameSource _displayNameSource = displayNameSource;
        private readonly DisplayNameOverride _displayNameOverride = displayNameOverride;
        public JsonNamingPolicy? NamingPolicy => _namingPolicy;
        public bool UsePropertyNameAsSource => _displayNameSource == DisplayNameSource.Name;
        public bool OverridePropertiesDisplayName => _displayNameOverride == DisplayNameOverride.Yes;

        public enum DisplayNameSource
        {
            Name,
            DisplayName
        }

        public enum DisplayNameOverride
        {
            No,
            Yes
        }
    }

    // The PWADisplayNamePropertyDescriptor provides a displayname wrapper around the specified PropertyDescriptor. 
    // See https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.propertydescriptor?view=net-8.0
    internal sealed class PWADisplayNamePropertyDescriptor(PropertyDescriptor pd, JsonNamingPolicy? namingPolicy, bool usePropertyName) : PropertyDescriptor(pd)
    {
        private readonly PropertyDescriptor _pd = pd;
        private readonly DisplayNameAttribute _displayNameAttr = new(MyJsonNamingPolicy.ConvertString(namingPolicy, usePropertyName ? pd.Name : pd.DisplayName));

        public override AttributeCollection Attributes => AppendAttributeCollection(new AttributeCollection(_pd.Attributes.OfType<Attribute>().Where(a => a is not DisplayNameAttribute).ToArray()), _displayNameAttr);
        protected override void FillAttributes(System.Collections.IList attributeList)
        {
            List<object> baseAttributes = [];
            base.FillAttributes(baseAttributes);
            foreach (var a in baseAttributes)
            {
                if (a is not DisplayNameAttribute)
                {
                    attributeList.Add(a);
                }
            }
            attributeList.Add(_displayNameAttr);
        }
        public override Type ComponentType => _pd.ComponentType;
        public override TypeConverter Converter => _pd.Converter;
        public override object? GetEditor(Type editorBaseType) => _pd.GetEditor(editorBaseType);
        public override bool IsLocalizable => _pd.IsLocalizable;
        public override bool IsReadOnly => _pd.IsReadOnly;
        public override Type PropertyType => _pd.PropertyType;
        public override void AddValueChanged(object component, EventHandler handler) => _pd.AddValueChanged(component, handler);
        public override void RemoveValueChanged(object component, EventHandler handler) => _pd.RemoveValueChanged(component, handler);
        public override bool CanResetValue(object component) => _pd.CanResetValue(component);
        public override PropertyDescriptorCollection GetChildProperties(object? instance, Attribute[]? filter) => _pd.GetChildProperties(instance, filter);
        public override object? GetValue(object? component) => _pd.GetValue(component);
        public override void ResetValue(object component) => _pd.ResetValue(component);
        public override void SetValue(object? component, object? val) => _pd.SetValue(component, val);
        public override bool ShouldSerializeValue(object component) => _pd.ShouldSerializeValue(component);
        public override bool SupportsChangeEvents => _pd.SupportsChangeEvents;


        // The following Utility methods create a new AttributeCollection
        // by appending the specified attributes to an existing collection.
        static public AttributeCollection AppendAttributeCollection(AttributeCollection existing, params Attribute[] newAttrs)
        {
            return new AttributeCollection(AppendAttributes(existing, newAttrs));
        }

        static public Attribute[] AppendAttributes(AttributeCollection existing, params Attribute[] newAttrs)
        {
            ArgumentNullException.ThrowIfNull(existing);

            newAttrs ??= [];

            Attribute[] attributes;

            Attribute[] newArray = new Attribute[existing.Count + newAttrs.Length];
            int actualCount = existing.Count;
            existing.CopyTo(newArray, 0);

            for (int idx = 0; idx < newAttrs.Length; idx++)
            {
                ArgumentNullException.ThrowIfNull(newAttrs[idx], nameof(newAttrs));

                // Check if this attribute is already in the existing
                // array.  If it is, replace it.
                bool match = false;
                for (int existingIdx = 0; existingIdx < existing.Count; existingIdx++)
                {
                    if (newArray[existingIdx].TypeId.Equals(newAttrs[idx].TypeId))
                    {
                        match = true;
                        newArray[existingIdx] = newAttrs[idx];
                        break;
                    }
                }

                if (!match)
                {
                    newArray[actualCount++] = newAttrs[idx];
                }
            }

            // If some attributes were collapsed, create a new array.
            if (actualCount < newArray.Length)
            {
                attributes = new Attribute[actualCount];
                Array.Copy(newArray, 0, attributes, 0, actualCount);
            }
            else
            {
                attributes = newArray;
            }

            return attributes;
        }
    }

    // TypeConverter for classes whose properties should be displayed with a specific JsonNamingPolicy in PropertyGrid editor.
    class PWADisplayNamePropertyTypeConverter(Type type) : ExpandableObjectConverter
    {
        private readonly Type _type = type;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
        {
            PropertyDescriptorCollection props = base.GetProperties(context, value, attributes);

            if (_type.GetCustomAttributes<PWADisplayNamePropertyTypeConverterParamsAttribute>().FirstOrDefault() is PWADisplayNamePropertyTypeConverterParamsAttribute converterParams)
            {
                List<PropertyDescriptor> newProps = new(props.Count);
                foreach (PropertyDescriptor prop in props)
                {
                    if (converterParams.OverridePropertiesDisplayName || !prop.Attributes.OfType<DisplayNameAttribute>().Any())
                    {
                        newProps.Add(new PWADisplayNamePropertyDescriptor(prop, converterParams.NamingPolicy, converterParams.UsePropertyNameAsSource));
                    }
                    else
                    {
                        newProps.Add(prop);
                    }
                }
                return new PropertyDescriptorCollection([.. newProps], true);
            }

            return props;
        }
    }
}
