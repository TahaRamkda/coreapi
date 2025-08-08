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
        public string LanguageCode { get; set; } = string.Empty;
        public BodyDto Body { get; set; }
        public List<CardDto> Cards { get; set; }

        public class BodyDto
        {
            public BodyDto()
            {
                DynamicValues = new List<KeyValue>();
            }

            public string Type { get; set; } = "body";
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

            public HeaderDto Header { get; set; }
            public CardBodyDto Body { get; set; }
            public List<ButtonDto> Buttons { get; set; }
        }

        public class HeaderDto
        {
            public HeaderDto()
            {
                DynamicValue = new KeyValue();
            }
            public int Format { get; set; }
            public string Text { get; set; } = string.Empty;
            public int MediaId { get; set; }
            public KeyValue DynamicValue { get; set; }
        }

        public class CardBodyDto
        {
            public CardBodyDto()
            {
                DynamicValues = new List<KeyValue>();
            }
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

        public class CaraouselParameter
        {
            public string ParamName { get; set; }
            public string ParamDefaultValue { get; set; }
            public int Sequence { get; set; }
            public int ParamType { get; set; }
            public int PersonalizationType { get; set; }
            public string PersonalizationField { get; set; }
            public string PersonalizationDefaultValue { get; set; }
        }


        public class CaraouselButton
        {
            public string ButtonText { get; set; } = string.Empty;
            public string ButtonValue { get; set; } = string.Empty;
            public int ButtonType { get; set; }
            public int Sequence { get; set; }
            public int SytemActionId { get; set; }
            public int ActionId { get; set; }
            public int ActionType { get; set; }
            public int CardNumber { get; set; }
        }

        public class CarouselScreen
        {
            public int Sequence { get; set; }
            public int HeaderType { get; set; }
            public int MediaId { get; set; }
            public int HeaderParamCount { get; set; }
            public string HeaderText { get; set; }
            public int BodyParamCount { get; set; }
            public string BodyText { get; set; }
            public string FooterText { get; set; }
        }
    }
}