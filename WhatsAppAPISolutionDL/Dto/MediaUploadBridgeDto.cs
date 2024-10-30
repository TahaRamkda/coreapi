using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class MediaUploadBridgeDto
    {
        public MediaUploadBridgeDto() {

            medias = new List<Media>();
        }
        public string phoneId {  get; set; }
        public List<Media> medias {  get; set; }
        public partial class Media
        {
            public string id { get; set; }
            public string url { get; set; }
        }
    }
}
