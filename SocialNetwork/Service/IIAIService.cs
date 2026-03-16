using SocialNetwork.Common;

namespace SocialNetwork.Service
{
    public interface IIAIService
    {
        Task<PayLoad<object>> AIImage(IFormFile file);
    }
}
