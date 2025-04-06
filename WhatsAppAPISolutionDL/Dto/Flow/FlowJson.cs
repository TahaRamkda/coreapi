using Newtonsoft.Json;

namespace WhatsAppAPISolutionDL.Dto.Flow
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class FlowJson
    {
        public FlowJson()
        {
            screens = new List<Screen>();
        }

        public string version { get; set; }
        public string data_api_version { get; set; }
        public Dictionary<string, List<string>> routing_model { get; set; }

        public List<Screen> screens { get; set; }

        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        public class Screen
        {
            public Screen()
            {

            }

            public string id { get; set; }
            public string title { get; set; }
            public bool? terminal { get; set; }
            public Layout layout { get; set; }
        }

        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        public class Layout
        {
            public Layout()
            {
                children = new List<LayoutChildren>();
            }

            public string type { get; set; }
            public List<LayoutChildren> children { get; set; }
        }

        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        public class LayoutChildren
        {
            public LayoutChildren()
            {
                children = new List<Children>();
            }

            public string type { get; set; }
            public string name { get; set; }
            public List<Children> children { get; set; }
        }

        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        public class Children
        {
            public Children()
            {
            }

            public string type { get; set; }
            public string label { get; set; }
            public string text { get; set; }
            public string name { get; set; }
            public bool? required { get; set; }

            [JsonProperty("min-selected-items")]
            public int? minselection { get; set; }

            [JsonProperty("max-selected-items")]
            public int? maxselection { get; set; }

            [JsonProperty("data-source")]
            public List<DataSource> datasource { get; set; }

            [JsonProperty("on-click-action")]
            public OnClickAction onClickAction { get; set; }
        }

        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        public class DataSource
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string metadata { get; set; }
        }

        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        public class OnClickAction
        {
            public string name { get; set; }
            public Next next { get; set; }
            public Dictionary<string, string> payload { get; set; }

            [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
            public class Next
            {
                public string type { get; set; }
                public string name { get; set; }
            }
        }
    }
}
