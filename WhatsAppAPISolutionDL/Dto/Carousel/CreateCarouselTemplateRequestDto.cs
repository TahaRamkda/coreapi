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

       // public string ClientId { get; set; }
        public int SenderNameId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public HeaderComponent Header { get; set; }
        public List<CardDto> Cards { get; set; }

        public class CardDto
        {
            public CardDto()
            {
                Buttons = new List<ButtonDto>();
            }
            public string Type { get; set; } // "header" or "buttons"
            public HeaderComponent Header { get; set; }
            public BodyComponent Body { get; set; }
            public List<ButtonDto> Buttons { get; set; }
        }

        public class HeaderComponent
        {
            public HeaderComponent()
            {
                DynamicValue = new KeyValue();
            }

            public int Format { get; set; }
            public string Text { get; set; } = string.Empty;
            public int MediaId { get; set; }
            public KeyValue DynamicValue { get; set; }
        }

        public class BodyComponent
        {
            public BodyComponent()
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
            public int ButtonType { get; set; }
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
//public async Task<UResponseWithID> AddTemplateAsync(int clientId, int userId, TemplateDto model)
//{
//    //Replace empty space with _
//    model.Name = model.Name.Replace(" ", "_").ToLower().Trim();

//    //Check if template name already exists
//    var templateNameExist = await _dbContext.Templates
//        .Where(x => x.ClientId == clientId
//        && x.SenderId == model.SenderNameId
//        //&& x.RecordStatus != -1
//        && x.TemplateName != null
//        && x.TemplateName.ToLower() == model.Name.ToLower()
//        //&& x.Language != null
//        //&& x.Language.ToLower() == model.Language.ToLower()).
//        ).FirstOrDefaultAsync();

//    if (templateNameExist != null)
//        return new UResponseWithID { Message = "Template with same name already exist" };

//    //Globals 
//    Regex regex = new Regex(CommonHelper.DynamicPattern);
//    int headerType = 0;
//    string headerText = String.Empty;
//    int headerTextCount = 0;
//    string bodyText = String.Empty;
//    int bodyTextCount = 0;
//    string footerText = String.Empty;
//    List<TemplateDto.TemplateParameter> parameters = new List<TemplateDto.TemplateParameter>();
//    List<TemplateDto.TemplateButton> buttons = new List<TemplateDto.TemplateButton>();

//    //If header exist
//    if (model.Header != null)
//    {
//        if (model.Header.Format < 0)
//            return new UResponseWithID { Message = "Header format not mentioned" };

//        headerType = model.Header.Format;
//        var headerFormat = (TemplateHeaderEnum)model.Header.Format;
//        var headerMediaTypes = new List<TemplateHeaderEnum> { TemplateHeaderEnum.IMAGE, TemplateHeaderEnum.VIDEO, TemplateHeaderEnum.DOCUMENT };
//        if (headerMediaTypes.Contains(headerFormat))
//        {
//            //Media related validations
//            if (model.MediaId <= 0)
//                return new UResponseWithID { Message = "Media is required when header type is not text" };

//            var mediaDetail = await _dbContext.Medias.FindAsync(model.MediaId);
//            if (mediaDetail == null || string.IsNullOrEmpty(mediaDetail.MediaPath))
//                return new UResponseWithID { Message = "Media not exist" };

//            if (mediaDetail.SenderNameId != model.SenderNameId)
//                return new UResponseWithID { Message = "Media does not exist for this sender" };

//            var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType(headerFormat, mediaDetail.FileExtension);
//            if (!allowedMedia)
//                return new UResponseWithID { Message = $"Not allowed media for header type - {headerFormat}" };

//        }

//        if (headerFormat == TemplateHeaderEnum.TEXT)
//        {
//            if (String.IsNullOrWhiteSpace(model.Header.Text))
//                return new UResponseWithID { Message = "Header text is required" };

//            model.Header.Text = model.Header.Text.Trim();
//            headerText = model.Header.Text;

//            MatchCollection matches = regex.Matches(model.Header.Text);
//            if (matches.Count > 0)
//            {
//                if (model.Header.DynamicValue == null || String.IsNullOrWhiteSpace(model.Header.DynamicValue.ParamName) || String.IsNullOrWhiteSpace(model.Header.DynamicValue.ParamValue))
//                    return new UResponseWithID { Message = "Header default parameter is required" };

//                if (matches.Count > 1)
//                    return new UResponseWithID { Message = "Only one header parameter is allowed" };

//                if (matches[0].Value != model.Header.DynamicValue.ParamName)
//                    return new UResponseWithID { Message = "Headere parameter passed does not match with header text" };

//                headerTextCount = matches.Count;
//                parameters.Add(new TemplateDto.TemplateParameter
//                {
//                    ParamType = (int)TemplateParamEnum.Header,
//                    ParamName = model.Header.DynamicValue.ParamName,
//                    ParamDefaultValue = model.Header.DynamicValue.ParamValue,
//                    Sequence = 0
//                });
//            }
//        }
//    }

//    //If body exist
//    if (model.Body != null)
//    {
//        if (String.IsNullOrWhiteSpace(model.Body.Text))
//            return new UResponseWithID { Message = "Body text is required" };

//        model.Body.Text = model.Body.Text.Trim();
//        bodyText = model.Body.Text;
//        MatchCollection matches = regex.Matches(model.Body.Text);
//        if (matches.Count > 0)
//        {
//            if (model.Body.DynamicValues == null || model.Body.DynamicValues.Count == 0)
//                return new UResponseWithID { Message = "Body text parameter is required" };

//            var duplicate = matches.Select(x => x.Value).GroupBy(item => item).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
//            if (duplicate.Any())
//                return new UResponseWithID { Message = "Cannot have duplicate parameters in body." };

//            if (matches.Count != model.Body.DynamicValues.Count)
//                return new UResponseWithID { Message = "Body text parameters is not matching with body text count" };

//            var passedEmpty = model.Body.DynamicValues.Any(x => String.IsNullOrWhiteSpace(x.ParamName) || String.IsNullOrWhiteSpace(x.ParamValue));
//            if (passedEmpty)
//                return new UResponseWithID { Message = "Parameter name or value is not passed correctly in body" };

//            var matchValues = matches.Select(x => x.Value).ToList();
//            var bodyParams = model.Body.DynamicValues.Select(x => x.ParamName).ToList();

//            var areEqual = matchValues.All(item => bodyParams.Contains(item));
//            if (!areEqual)
//                return new UResponseWithID { Message = "Body parameters passed does not match with body text" };

//            bodyTextCount = matches.Count;
//            for (int i = 0; i < model.Body.DynamicValues.Count; i++)
//            {
//                var value = model.Body.DynamicValues[i];
//                parameters.Add(new TemplateDto.TemplateParameter
//                {
//                    ParamType = (int)TemplateParamEnum.Body,
//                    ParamName = value.ParamName,
//                    ParamDefaultValue = value.ParamValue,
//                    Sequence = i
//                });
//            }
//        }
//    }

//    if (model.Footer != null)
//        footerText = model.Footer.Text.Trim();

//    if (model.Buttons != null && model.Buttons.Count > 0)
//    {
//        for (int i = 0; i < model.Buttons.Count; i++)
//        {
//            var button = model.Buttons[i];
//            if (button.ActionType == (int)ActionTypeEnum.TEMPLATE && button.ActionId <= 0)
//                return new UResponseWithID { Message = $"Template id required in action id when action type is {(int)ActionTypeEnum.TEMPLATE}" };

//            button.ButtonValue = (button.ButtonValue ?? "").Trim();

//            if (!String.IsNullOrWhiteSpace(button.ButtonValue))
//            {
//                MatchCollection matches = regex.Matches(button.ButtonValue);
//                if (matches.Count > 0)
//                {
//                    if (button.ButtonType != (int)ButtonTypeEnum.URL)
//                        return new UResponseWithID { Message = $"Dynamic parameter not allowed in button type {(ButtonTypeEnum)button.ButtonType}" };

//                    if (button.DynamicValue == null || String.IsNullOrWhiteSpace(button.DynamicValue.ParamName) || String.IsNullOrWhiteSpace(button.DynamicValue.ParamValue))
//                        return new UResponseWithID { Message = "Button default parameter is required" };

//                    if (matches.Count > 1)
//                        return new UResponseWithID { Message = "Only one button parameter is allowed" };

//                    if (matches[0].Value != button.DynamicValue.ParamName)
//                        return new UResponseWithID { Message = $"Button - {button.ButtonText} parameter passed does not match with button value" };

//                    parameters.Add(new TemplateDto.TemplateParameter
//                    {
//                        ParamType = (int)TemplateParamEnum.Button,
//                        ParamName = button.DynamicValue.ParamName,
//                        ParamDefaultValue = button.DynamicValue.ParamValue,
//                        Sequence = button.Sequence //Assigning button sequence
//                    });
//                }
//            }

//            buttons.Add(new TemplateDto.TemplateButton
//            {
//                ButtonType = button.ButtonType,
//                ButtonText = button.ButtonText,
//                ButtonValue = button.ButtonValue,
//                ActionId = button.ActionId,
//                ActionType = button.ActionType,
//                Sequence = button.Sequence,
//                SytemActionId = button.SytemActionId
//            });
//        }
//    }

//    var parameterJson = JsonConvert.SerializeObject(parameters);
//    var buttonJson = JsonConvert.SerializeObject(buttons);

//    var responseList = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={clientId}, @SenderId={model.SenderNameId},  @TemplateName={model.Name},@Category={model.Category}, @Language={model.Language}, @HeaderType={headerType}, @HeaderParamCount={headerTextCount}, @HeaderText={headerText},@MediaId={model.MediaId}, @BodyText={bodyText}, @BodyParamCount={bodyTextCount}, @FooterText={footerText}, @ButtonsJson={buttonJson},@ParametersJson={parameterJson}, @ActionBy={userId}").ToListAsync();

//    await _cacheService.RemoveByPrefix(CacheKeys.TEMPLATE_PATTERN_KEY);

//    if (responseList == null || !responseList.Any())
//        return new UResponseWithID { Message = "Cannot add template" };

//    var response = responseList[0];
//    if (response.Status <= 0)
//        return new UResponseWithID { Message = response.Message };

//    //Push template to facebook
//    return await PushTemplateToFacebook(clientId, response.Id);
//}
