//#define PWA_EXTRA_PROPERTIES

using System.ComponentModel;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using static PWAHelper.PWADisplayNamePropertyTypeConverterParamsAttribute;

//See
//- https://w3c.github.io/manifest
//- https://json.schemastore.org/web-manifest
//- https://developer.mozilla.org/en-US/docs/Web/Manifest
namespace PWAHelper
{
    [TypeConverter(typeof(PWAEnumEditorConverter))]
    [PWAEnumEditorTextEditable(ReadOnly = true)]
    [JsonNamingPolicyEnum(Policy = "KebabCaseLower", AllowIntegerValues = false)]
    [JsonConverter(typeof(PWAJsonStringEnumConverter<PWADisplay>))]
    internal enum PWADisplay
    {
        Fullscreen,
        Standalone,
        MinimalUi,
        Browser
    };

    [TypeConverter(typeof(PWAEnumEditorConverter))]
    [PWAEnumEditorTextEditable(ReadOnly = true)]
    [JsonNamingPolicyEnum(Policy = "KebabCaseLower", AllowIntegerValues = false)]
    [JsonConverter(typeof(PWAJsonStringEnumConverter<PWAOrientation>))]
    internal enum PWAOrientation
    {
        Any,
        Natural,
        Landscape,
        LandscapePrimary,
        LandscapeSecondary,
        Portrait,
        PortraitPrimary,
        PortraitSecondary
    };

    [TypeConverter(typeof(PWAEnumEditorConverter))]
    [PWAEnumEditorTextEditable(ReadOnly = true)]
    [JsonNamingPolicyEnum(Policy = "KebabCaseLower", AllowIntegerValues = false)]
    [JsonConverter(typeof(PWAJsonStringEnumConverter<PWADir>))]
    internal enum PWADir
    {
        Auto,
        Ltr,
        Rtl
    };

    [Editor(typeof(PWAEnumFlagsEditorUI), typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter(typeof(PWASpacedEnumFlagsEditorConverter))]
    [PWAEnumEditorTextEditable(ReadOnly = true)]
    [JsonNamingPolicyEnum(Policy = "SpaceCaseLower", AllowIntegerValues = false)]
    [JsonConverter(typeof(PWASpacedEnumFlagsJsonConverter<PWAImagePurpose>))]
    [Flags]
    internal enum PWAImagePurpose
    {
        Any = 1,
        Monochrome = 2,
        Maskable = 4
    };

    [PWADisplayNamePropertyTypeConverterParams("SnakeCaseLower", DisplayNameSource.Name, DisplayNameOverride.No)]
    [TypeConverter(typeof(PWADisplayNamePropertyTypeConverter))]
    internal class PWAIcon
    {
        public string Src { get; set; } = "";
        public string Sizes { get; set; } = "";
        public string Type { get; set; } = "";
        public PWAImagePurpose Purpose { get; set; } = PWAImagePurpose.Any;

        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(Src) ? Src : "icon";
        }
    };

    [PWADisplayNamePropertyTypeConverterParams("SnakeCaseLower", DisplayNameSource.Name, DisplayNameOverride.No)]
    [TypeConverter(typeof(PWADisplayNamePropertyTypeConverter))]
    internal class PWAShortcut
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
        public string ShortName { get; set; } = "";    //optional
        public string Description { get; set; } = "";//optional

        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAIcon> Icons { get; set; } = [];//optional

        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(Name) ? Name : "shortcut";
        }
    };

#if PWA_EXTRA_PROPERTIES
    [TypeConverter(typeof(PWAEnumEditorConverter))]
    [PWAEnumEditorTextEditable(ReadOnly = true)]
    [JsonNamingPolicyEnum(Policy = "KebabCaseLower", AllowIntegerValues = false)]
    [JsonConverter(typeof(PWAJsonStringEnumConverter<PWADisplayOverride>))]
    internal enum PWADisplayOverride
    {
        Fullscreen,
        Standalone,
        MinimalUi,
        Browser,
        WindowControlsOverlay
    };

    [TypeConverter(typeof(PWAEnumEditorConverter))]
    [PWAEnumEditorTextEditable(ReadOnly = true)]
    [JsonNamingPolicyEnum(Policy = "SnakeCaseLower", AllowIntegerValues = false)]
    [JsonConverter(typeof(PWAJsonStringEnumConverter<PWAApplicationPlatform>))]
    internal enum PWAApplicationPlatform
    {
        ChromeWebStore,
        Play,
        Itunes,
        Windows
    };

    [PWADisplayNamePropertyTypeConverterParams("SnakeCaseLower", DisplayNameSource.Name, DisplayNameOverride.No)]
    [TypeConverter(typeof(PWADisplayNamePropertyTypeConverter))]
    internal class PWAScreenshot
    {
        public string Src { get; set; } = "";
        public string Sizes { get; set; } = "";
        public string Type { get; set; } = "";
        public string Label { get; set; } = "";
        public string Platform { get; set; } = "";
        public string FormFactor { get; set; } = "";
        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(Src) ? Src : "screenshot";
        }
    };

    [PWADisplayNamePropertyTypeConverterParams("SnakeCaseLower", DisplayNameSource.Name, DisplayNameOverride.No)]
    [TypeConverter(typeof(PWADisplayNamePropertyTypeConverter))]
    internal class PWAProtocolHandler
    {
        public string Protocol { get; set; } = "";
        public string Url { get; set; } = "";

        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(Url) ? Url : "protocol handler";
        }
    };

    [PWADisplayNamePropertyTypeConverterParams("SnakeCaseLower", DisplayNameSource.Name, DisplayNameOverride.No)]
    [TypeConverter(typeof(PWADisplayNamePropertyTypeConverter))]
    internal class PWAApplicationFingerprint
    {
        public string Type { get; set; } = "";
        public string Value { get; set; } = "";

        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return "fingerprint";
        }
    }

    [PWADisplayNamePropertyTypeConverterParams("SnakeCaseLower", DisplayNameSource.Name, DisplayNameOverride.No)]
    [TypeConverter(typeof(PWADisplayNamePropertyTypeConverter))]
    internal class PWAApplication
    {
        public PWAApplicationPlatform Platform { get; set; } = PWAApplicationPlatform.ChromeWebStore;
        public string Url { get; set; } = "";
        public string Id { get; set; } = "";
        public string MinVersion { get; set; } = "";

        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAApplicationFingerprint> Fingerprints { get; set; } = [];

        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return JsonNamingPolicy.SnakeCaseLower.ConvertName(Platform.ToString());
        }
    };
#endif  //PWA_EXTRA_PROPERTIES

    [PWADisplayNamePropertyTypeConverterParams("SnakeCaseLower", DisplayNameSource.Name, DisplayNameOverride.No)]
    [TypeConverter(typeof(PWADisplayNamePropertyTypeConverter))]
    internal class PWAManifest
    {
        public string Id { get; set; } = "./index.html";
        public string Name { get; set; } = "";
        public string ShortName { get; set; } = "";
        public string Description { get; set; } = "";   //optional?
        public string Lang { get; set; } = CultureInfo.CurrentCulture.Name;
        public string StartUrl { get; set; } = "./index.html";
        public string Scope { get; set; } = "./";


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public PWADir Dir { get; set; } = PWADir.Auto;


        [JsonConverter(typeof(PWAColorJsonConverter))]
        public Color ThemeColor { get; set; } = Color.White;


        [JsonConverter(typeof(PWAColorJsonConverter))]
        public Color BackgroundColor { get; set; } = Color.White;

        public PWAOrientation Orientation { get; set; } = PWAOrientation.Any;

        public PWADisplay Display { get; set; } = PWADisplay.Browser;


        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAIcon> Icons { get; set; } = [];


        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAShortcut> Shortcuts { get; set; } = [];

#if PWA_EXTRA_PROPERTIES
        public bool PreferRelatedApplications { get; set; } = false;
        
        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAApplication> RelatedApplications { get; set; } = [];

        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWADisplayOverride> DisplayOverride { get; set; } = [];

        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAScreenshot> Screenshots { get; set; } = [];
        
        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAProtocolHandler> ProtocolHandlers { get; set; } = [];
#endif  //PWA_EXTRA_PROPERTIES

        public PWAManifest()
        {
            foreach (string resolution in IconResolutions)
            {
                var iconName = Path.Combine(".\\icons", $"icon-{resolution}.png").Replace("\\", "/");
                Icons.Add(new PWAIcon { Src = iconName, Sizes = resolution, Type = "image/png" });
            }
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, s_defaultSerializationOptions);
        }

        public static PWAManifest? FromJson(string json)
        {
            return JsonSerializer.Deserialize<PWAManifest>(json, s_defaultDeserializationOptions);
        }

        public static readonly string[] IconResolutions = [
                    "48x48",
                    "72x72",
                    "96x96",
                    "144x144",
                    "168x168",
                    "192x192",
                    "256x256",
                    "512x512"
        ];

        private static readonly JsonSerializerOptions s_defaultSerializationOptions = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        private static readonly JsonSerializerOptions s_defaultDeserializationOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
    }
}
