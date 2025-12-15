using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Common;
using SocialNetwork.Service;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _postService.FindAll(name, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<PostDTO>> Add (PostDTO data)
        {
            return await _postService.Add(data);
        }
    }
}