namespace WhatsAppAPISolutionDL.Dto.Order.KFG
{
    public class KFGOrderPostingResult
    {
        public Response remoteResponse { get; set; }

        public class Response
        {
            public string remoteOrderId { get; set; }
            public string acceptanceTime { get; set; }
        }
    }
}
