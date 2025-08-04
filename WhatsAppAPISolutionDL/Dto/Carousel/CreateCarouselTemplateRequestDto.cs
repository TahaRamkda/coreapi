using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.Dto.Carousel
{
    public class CreateCarouselTemplateRequestDto
    {
        public CreateCarouselTemplateRequestDto()
        {
            Cards = new List<CardDto>();
        }

        public int Id { get; set; }
        public string ClientId { get; set; }
        public int SenderNameId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public HeaderComponent Header { get; set; }
        public List<CardDto> Cards { get; set; }

        public class BodyDto
        {
            public BodyDto()
            {
                DynamicValues = new List<KeyValue>();
            }

            public string Type { get; set; } = "body";
            public int Format { get; set; }
            public string Text { get; set; } = string.Empty;
            public List<KeyValue> DynamicValues { get; set; }
        }

        public class CardDto
        {
            public CardDto()
            {
                Header = new HeaderDto();
                Body = new CardBodyDto();
                Buttons = new List<ButtonDto>();
            }
            public string Type { get; set; } // "header" or "buttons"
            public HeaderDto Header { get; set; }
            public BodyDto Body { get; set; }
            public List<ButtonDto> Buttons { get; set; }
        }

        public class HeaderComponent
        {
            public HeaderComponent()
            {
                DynamicValue = new KeyValue();
            }
            public string type { get; set; }    
            public int Format { get; set; }
            public string Text { get; set; } = string.Empty;
            public int MediaId { get; set; }
            public KeyValue DynamicValue { get; set; }
        }

        public class BodyDto
        {
            public BodyDto()
            {
                DynamicValues = new List<KeyValue>();
            }
            public int Format { get; set; }
            public string Text { get; set; } = string.Empty;
            public List<KeyValue> DynamicValues { get; set; }
        }

        public class ButtonDto
        {
            public ButtonDto()
            {
                DynamicValue = new KeyValue();
            }

            public string ButtonText { get; set; } = string.Empty;
            public string ButtonValue { get; set; } = string.Empty;
            public int ButtonType { get; set; } // 1 = quick reply, 2 = url, 3 = phone_number
            public int Sequence { get; set; }
            public int SytemActionId { get; set; }
            public int ActionType { get; set; }
            public int ActionId { get; set; }
            public KeyValue DynamicValue { get; set; }
        }

        public class KeyValue
        {
            public string ParamName { get; set; }
            public string ParamValue { get; set; }
        }
    }
}
