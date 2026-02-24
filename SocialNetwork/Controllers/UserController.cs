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
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;
        public UserController(IUserService user)
        {
            _user = user;
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _user.FindAll(name, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(uploadImageUser))]
        public async Task<PayLoad<object>> uploadImageUser([FromForm]ImageUserUpload data)
        {
            return await _user.uploadImageUser(data);
        }

        [HttpGet]
        [Route(nameof(FindAlluploadImageUser))]
        public async Task<PayLoad<object>> FindAlluploadImageUser()
        {
            return await _user.FindAlluploadImageUser();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<UserDTO>> Add([FromForm]UserDTO data)
        {
            return await _user.Add(data);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Login))]
        public async Task<PayLoad<object>> Login (LoginDTO data)
        {
            return await _user.Login(data);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(ForgotPassword))]
        public async Task<PayLoad<object>> ForgotPassword(ForgotPasswordDTO data)
        {
            return await _user.ForgotPassword(data);
        }

        [HttpPut]
        [Route(nameof(UpdateProfile))]
        public async Task<PayLoad<object>> UpdateProfile([FromForm] UpdateProfileDTO data)
        {
            return await _user.UpdateProfile(data);
        }

        [HttpGet]   
        [Route(nameof(GetProfile))]
        public async Task<PayLoad<object>> GetProfile()
        {   
            return await _user.GetProfile();
        }

        [HttpDelete]
        [Route(nameof(DeleteImageUser))]
        public async Task<PayLoad<object>> DeleteImageUser(int imageId)
        {
            return await _user.DeleteImageUser(imageId);
        }
    }
}
