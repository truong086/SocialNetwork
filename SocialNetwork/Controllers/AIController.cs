using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Common;
using SocialNetwork.Service;

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IIAIService _ai;
        public AIController(IIAIService ai)
        {
            _ai = ai;
        }

        [HttpPost]
        [Route(nameof(AIImage))]
        public async Task<PayLoad<object>> AIImage(IFormFile file)
        {
            return await _ai.AIImage(file);
        }
    }
}
