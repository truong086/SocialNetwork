using Microsoft.AspNetCore.Authentication;
using SocialNetwork.ViewModel;
using System.Security.Claims;

namespace SocialNetwork.Service
{
    public class UserNameLoginService : IUserNameLoginService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserNameLoginService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.SignOutAsync();
        }

        public string name()
        {
            string value = string.Empty;
            if (_httpContextAccessor != null)
            {
                value = _httpContextAccessor.HttpContext.User.FindFirstValue(Status.IDAUTHENTICATION);
            }

            return value;
        }
    }
}
