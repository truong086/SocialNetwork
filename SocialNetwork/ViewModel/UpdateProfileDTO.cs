namespace SocialNetwork.ViewModel
{
    public class UpdateProfileDTO
    {
        public string? fullname { get; set; }
        public IFormFile? image { get; set; }
        public string? signature_name { get; set; }
        public string? signature_font { get; set; }
        public int? signature_size { get; set; }
    }
}