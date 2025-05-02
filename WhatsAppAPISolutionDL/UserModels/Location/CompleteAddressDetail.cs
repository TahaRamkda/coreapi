using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Location
{
    public class CompleteAddressDetail
    {
        
        public string Block { get; set; }
        public string Street { get; set; }
        public string BuildingName { get; set; }
        public int FlatNo { get; set; }
        public int FloorNo { get; set; }
        public string Governate { get; set; }
        public string AreaName {  get; set; }
    }
}
