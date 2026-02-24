using System.Security.Cryptography;
using System.Text;

namespace SocialNetwork.Common
{
    public enum HashedPasswordFormat
    {
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }
    public static class EncryptionHelper
    {
        // Hàm mã hóa mật khẩu bằng BCrypt (khuyên dùng)
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        // Hàm xác minh mật khẩu bằng BCrypt
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        // Giữ lại hàm cũ để hỗ trợ verify password cũ đã hash bằng SHA256 (migration)
        public static string CreatePasswordHash(string password, string key, HashedPasswordFormat hashedPasswordFormat = HashedPasswordFormat.SHA256)
        {
            string ConverPasswordAndKey = string.Concat(password, key);
            HashAlgorithm hashAlgorithm = hashedPasswordFormat switch
            {
                HashedPasswordFormat.SHA512 => SHA512.Create(),
                HashedPasswordFormat.SHA384 => SHA384.Create(),
                HashedPasswordFormat.SHA256 => SHA256.Create(),
                HashedPasswordFormat.SHA1 => SHA1.Create(),
                _ => throw new NotSupportedException("Not supported format")
            };

            if (hashAlgorithm == null)
                throw new ArgumentException("Null");

            var mahoa = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(ConverPasswordAndKey));
            return BitConverter.ToString(mahoa).Replace("-", "");
        }
    }
}
