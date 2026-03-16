using SocialNetwork.Common;

namespace SocialNetwork.Service
{
    public class IAIService : IIAIService
    {
        public async Task<PayLoad<object>> AIImage(IFormFile file)
        {
            try
            {
                var client = new HttpClient();

                var content = new MultipartFormDataContent();
                var steam = file.OpenReadStream();
                content.Add(new StreamContent(steam), "file", file.FileName);

                var res = await client.PostAsync("http://127.0.0.1:5000/detect-plate", content);

                var json = await res.Content.ReadAsStringAsync();

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = json
                }));
            }catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
