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
        public int Status { get; set; }

        public List<FlowScreenDTO> FlowScreens { get; set; } = new();
    }

    public class FlowScreenDTO
    {

        public string Name { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string ScreenButtonText { get; set; }
        public int RedirectionType { get; set; }
        public string RedirectionScreen { get; set; }
        public List<FlowChildrenDTO> FlowChildren { get; set; } = new();
    }

    public class FlowChildrenDTO
    {

        public string Name { get; set; }
        public string Text { get; set; }
        public int Type { get; set; }
        public bool Required { get; set; }

        public List<FlowOptionDTO> FlowOptions { get; set; } = new();
    }

    public class FlowOptionDTO
    {
        public string OptionText { get; set; }
    }
}
