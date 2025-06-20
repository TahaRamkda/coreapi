using System.Collections.Generic;
using Newtonsoft.Json;

namespace WhatsAppAPISolutionDL.Dto.TemplateAnalytics
{
    public class TemplateAnalyticsBridgeResponseDto
    {
        [JsonProperty("granularity")]
        public string? Granularity { get; set; }

        [JsonProperty("productType")]
        public string? ProductType { get; set; }

        [JsonProperty("dataPoints")]
        public List<DataPoint>? DataPoints { get; set; }
    }

    public class DataPoint
    {
        [JsonProperty("templateId")]
        public string? TemplateId { get; set; }

        [JsonProperty("start")]
        public long? Start { get; set; }

        [JsonProperty("end")]
        public long? End { get; set; }

        [JsonProperty("sent")]
        public int? Sent { get; set; }

        [JsonProperty("delivered")]
        public int? Delivered { get; set; }

        [JsonProperty("read")]
        public int? Read { get; set; }

        [JsonProperty("cost")]
        public List<Cost>? Cost { get; set; }

        [JsonProperty("clicked")]
        public List<Clicked>? Clicked { get; set; }
    }

    public class Cost
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("value")]
        public double? Value { get; set; }
    }

    public class Clicked
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("buttonContent")]
        public string? ButtonContent { get; set; }

        [JsonProperty("count")]
        public int? Count { get; set; }
    }
}
