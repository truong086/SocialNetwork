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
        public async Task<PayLoad<object>> FindAll(string? name, int? category, int page = 1, int pageSize = 20)
        {
            return await _postService.FindAll(name, category, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllPostByUser))]
        public async Task<PayLoad<object>> FindAllPostByUser(string? name, int? category, int page = 1, int pageSize = 20)
        {
            return await _postService.FindAllPostByUser(name, category, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<PostDTO>> Add (PostDTO data)
        {
            return await _postService.Add(data);
        }

        [HttpPost]
        [Route(nameof(addImageTestCloud))]
        public async Task<PayLoad<object>> addImageTestCloud(IFormFile data)
        {
            return await _postService.addImageTestCloud(data);
        }

        [HttpPost]
        [Route(nameof(AddEditImage))]
        public async Task<PayLoad<PostEditImageDTO>> AddEditImage([FromForm]PostEditImageDTO data)
        {
            return await _postService.AddEditImage(data);
        }

        [HttpPost]
        [Route(nameof(AddLike))]
        public async Task<PayLoad<object>> AddLike(int data)
        {
            return await _postService.AddLike(data);
        }
    }
}