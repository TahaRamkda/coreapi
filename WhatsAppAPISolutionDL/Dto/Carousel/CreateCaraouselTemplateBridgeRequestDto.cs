using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Carousel
{

    public class CreateCaraouselTemplateBridgeRequestDto
    {
        public CreateCaraouselTemplateBridgeRequestDto()
        {
            Cards = new List<CardDto>();
        }

        public string ClientId { get; set; }
        public string SenderNameId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string LanguageCode { get; set; }
        public BodyDto Body { get; set; }
        public List<CardDto> Cards { get; set; }

        public class CardDto
        {
            public CardDto()
            {
                Buttons = new List<ButtonDto>();
            }

            public HeaderDto Header { get; set; }
            public BodyDto Body { get; set; }
            public List<ButtonDto> Buttons { get; set; }
        }
        public class HeaderDto
        {
            public HeaderDto()
            {
            }

            public string Format { get; set; }
            public string MediaUrl { get; set; }
            public string Text { get; set; }
            public string Example { get; set; }
        }

        public class BodyDto
        {
            public BodyDto()
            {
                Examples = new List<string>();
            }

            public string Text { get; set; }
            public List<string> Examples { get; set; }
        }

        public class ButtonDto
        {
            public ButtonDto()
            {

            }

            public string Type { get; set; }
            public string Text { get; set; }
            public string PhoneNumber { get; set; }
            public string Url { get; set; }
            public string Example { get; set; }
        }
    }
}

