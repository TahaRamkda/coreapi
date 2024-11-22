using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMessageService messageService,
            WhatsAppSolutionContext dbContext,
            ILogger<MessageController> logger)
        {
            _messageService = messageService;
            _dbContext = dbContext;
            _logger = logger;
        }

    }
}
