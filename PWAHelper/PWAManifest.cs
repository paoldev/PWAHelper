//#define PWA_EXTRA_PROPERTIES

using System.ComponentModel;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

//See
//- https://w3c.github.io/manifest
//- https://json.schemastore.org/web-manifest
//- https://developer.mozilla.org/en-US/docs/Web/Manifest
namespace PWAHelper
{
    [TypeConverter(typeof(PWAEnumEditorConverter))]
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
    [JsonNamingPolicyEnum(Policy = "SpaceCaseLower", AllowIntegerValues = false)]
    [JsonConverter(typeof(PWASpacedEnumFlagsJsonConverter<PWAImagePurpose>))]
    [Flags]
    internal enum PWAImagePurpose
    {
        Any = 1,
        Monochrome = 2,
        Maskable = 4
    };

    internal class PWAIcon
    {
        public string src { get; set; } = "";
        public string sizes { get; set; } = "";
        public string type { get; set; } = "";
        public PWAImagePurpose purpose { get; set; } = PWAImagePurpose.Any;

        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(src) ? src : "icon";
        }
    };

    internal class PWAShortcut
    {
        public string name { get; set; } = "";
        public string url { get; set; } = "";
        public string short_name { get; set; } = "";    //optional
        public string description { get; set; } = "";//optional

        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAIcon> icons { get; set; } = [];//optional

        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(name) ? name : "shortcut";
        }
    };

#if PWA_EXTRA_PROPERTIES
    [TypeConverter(typeof(PWAEnumEditorConverter))]
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
    [JsonNamingPolicyEnum(Policy = "SnakeCaseLower", AllowIntegerValues = false)]
    [JsonConverter(typeof(PWAJsonStringEnumConverter<PWAApplicationPlatform>))]
    internal enum PWAApplicationPlatform
    {
        ChromeWebStore,
        Play,
        Itunes,
        Windows
    };

    internal class PWAScreenshot
    {
        public string src { get; set; } = "";
        public string sizes { get; set; } = "";
        public string type { get; set; } = "";
        public string label { get; set; } = "";
        public string platform { get; set; } = "";
        public string form_factor { get; set; } = "";
        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(src) ? src : "screenshot";
        }
    };

    internal class PWAProtocolHandler
    {
        public string protocol { get; set; } = "";
        public string url { get; set; } = "";

        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(url) ? url : "protocol handler";
        }
    };

    internal class PWAApplicationFingerprint
    {
        public string type { get; set; } = "";
        public string value { get; set; } = "";

        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return "fingerprint";
        }
    }

    internal class PWAApplication
    {
        public PWAApplicationPlatform platform { get; set; } = PWAApplicationPlatform.ChromeWebStore;
        public string url { get; set; } = "";
        public string id { get; set; } = "";
        public string min_version { get; set; } = "";

        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAApplicationFingerprint> fingerprints { get; set; } = new();

        //Override the string shown in PropertyGridEditor "collections" entries
        public override string ToString()
        {
            return JsonNamingPolicy.SnakeCaseLower.ConvertName(platform.ToString());
        }
    };
#endif  //PWA_EXTRA_PROPERTIES

    internal class PWAManifest
    {
        public string id { get; set; } = "./index.html";
        public string name { get; set; } = "";
        public string short_name { get; set; } = "";
        public string description { get; set; } = "";   //optional?
        public string lang { get; set; } = CultureInfo.CurrentCulture.Name;
        public string start_url { get; set; } = "./index.html";
        public string scope { get; set; } = "./";


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public PWADir dir { get; set; } = PWADir.Auto;


        [JsonConverter(typeof(PWAColorJsonConverter))]
        public Color theme_color { get; set; } = Color.White;


        [JsonConverter(typeof(PWAColorJsonConverter))]
        public Color background_color { get; set; } = Color.White;

        public PWAOrientation orientation { get; set; } = PWAOrientation.Any;

        public PWADisplay display { get; set; } = PWADisplay.Browser;


        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAIcon> icons { get; set; } = [];


        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAShortcut> shortcuts { get; set; } = [];

#if PWA_EXTRA_PROPERTIES
        public bool prefer_related_applications { get; set; } = false;
        
        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAApplication> related_applications { get; set; } = [];

        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWADisplayOverride> display_override { get; set; } = [];

        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAScreenshot> screenshots { get; set; } = [];
        
        [TypeConverter(typeof(PWAExpandableObjectEditorConverter))]
        public List<PWAProtocolHandler> protocol_handlers { get; set; } = [];
#endif  //PWA_EXTRA_PROPERTIES

        public PWAManifest()
        {
            foreach (string resolution in IconResolutions)
            {
                var iconName = Path.Combine(".\\icons", $"icon-{resolution}.png").Replace("\\", "/");
                icons.Add(new PWAIcon { src = iconName, sizes = resolution, type = "image/png" });
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
