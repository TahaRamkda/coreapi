using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Common
{
    public class WitAiResponseDto
    {
        public Dictionary<string, object> Entities { get; set; }
        public List<Intent> Intents { get; set; }
        public string Text { get; set; }
        public Traits Traits { get; set; }
    }
    public class Intent
    {
        public double Confidence { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Traits
    {
        [JsonProperty("wit$on_off")]
        public List<OnOffTrait> WitOnOff { get; set; }

        [JsonProperty("wit$sentiment")]
        public List<SentimentTrait> WitSentiment { get; set; }
    }

    public class OnOffTrait
    {
        public double Confidence { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public class SentimentTrait
    {
        public double Confidence { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
