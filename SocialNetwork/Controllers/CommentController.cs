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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        [Route(nameof(AddComment))]
        public async Task<PayLoad<object>> AddComment(CommentDTO data)
        {
            return await _commentService.AddComment(data);
        }
    }
}
