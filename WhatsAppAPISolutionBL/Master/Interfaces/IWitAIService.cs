namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IWitAIService
    {
        Task<string> SendContentToWitAi(string message);
    }
}
