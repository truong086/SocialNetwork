using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Common;
using SocialNetwork.Service;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _role;
        public RoleController(IRoleService role)
        {
            _role = role;
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<roleDTO>> Add (roleDTO roleDTO)
        {
            return await _role.Add (roleDTO);
        }

        [HttpGet]
        [Route (nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _role.FindAll (name, page, pageSize);
        }
    }
}
