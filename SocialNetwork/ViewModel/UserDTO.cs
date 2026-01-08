namespace SocialNetwork.ViewModel
{
    public class UserDTO
    {
        public string? phone { get; set; }
        public string? fullname { get; set; }
        public string? username { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? quocgia { get; set; }
        public IFormFile? image { get; set; }
        public int? role_id { get; set; }

    }
}
