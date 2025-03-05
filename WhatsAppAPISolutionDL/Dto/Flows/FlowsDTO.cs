using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.DTO.Survey
{

    public class FlowDTO
    {
        public int SenderId { get; set; }
        public int ModuleId { get; set; }
        public int ParentId { get; set; }
        public string FlowName { get; set; }
        public string FlowLanguage { get; set; }
        public bool PublishToFB { get; set; }
        public int FlowId { get; set; }
        public List<FlowScreenDTO> FlowScreens { get; set; } = new();
    }

    public class FlowScreenDTO
    {

        public string Name { get; set; }
        public string Title { get; set; }
        public string ScreenButtonText { get; set; }
        public List<FlowChildrenDTO> FlowChildren { get; set; } = new();
    }

    public class FlowChildrenDTO
    {
        public string Text { get; set; }
        public int Type { get; set; }
        public bool Required { get; set; }

        public List<FlowOptionDTO> FlowOptions { get; set; } = new();
    }

    public class FlowOptionDTO
    {
        public string OptionId { get; set; }
        public string OptionText { get; set; }
    }
}
