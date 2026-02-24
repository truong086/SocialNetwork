using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Clouds;
using SocialNetwork.Common;
using SocialNetwork.Models;
using SocialNetwork.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialNetwork.Service
{
    public class UserService : IUserService
    {
        private readonly DBContext _context;
        private IMapper _mapper;
        private Jwt _jwt;
        private Cloud _cloud;
        private readonly IUserNameLoginService _userNameLoginService;
        public UserService(DBContext context, IMapper mapper, IOptionsMonitor<Jwt> jwt, IOptions<Cloud> cloud, IUserNameLoginService userNameLoginService)
        {
            _context = context;
            _mapper = mapper;
            _jwt = jwt.CurrentValue;
            _cloud = cloud.Value;
            _userNameLoginService = userNameLoginService;

        }
        public async Task<PayLoad<UserDTO>> Add(UserDTO userDTO)
        {
            try
            {
                var checkName = _context.users.FirstOrDefault(x => (x.username == userDTO.username || x.email == userDTO.email) && !x.deleted);
                if (checkName != null)
                    return await Task.FromResult(PayLoad<UserDTO>.CreatedFail(Status.DATATONTAI));


                var roleData = checkRole(userDTO.role_id);
                if(roleData == null)
                {
                    roleData = _context.roles.FirstOrDefault(x => x.name.ToLower() == "User".ToLower() && !x.deleted);
                }
                var mapData = _mapper.Map<User>(userDTO);
                mapData.password = EncryptionHelper.HashPassword(mapData.password);
                
                mapData.roles = roleData;
                mapData.role_id = roleData == null ? null : roleData.id;
                if (userDTO.image != null)
                {
                    var cloudResult = uploadCloud.CloudInaryIFromAccount(userDTO.image, Status.SOCIAL + "_" + mapData.email, _cloud);
                    mapData.image = cloudResult.Link;
                    mapData.publicid = cloudResult.PublicId;
                }

                _context.users.Add(mapData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<UserDTO>.Successfully(userDTO));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<UserDTO>.CreatedFail(ex.Message));
            }
        }

        private role? checkRole(int? id)
        {
            if(id != null && id != 0)
            {
                var checkRoleNotNull = _context.roles.FirstOrDefault(x => x.id == id && !x.deleted);
                return checkRoleNotNull;
            }

            var checkRole = _context.roles.FirstOrDefault(x => x.name == "Admin" && !x.deleted);
            return checkRole;

        }
        public Task<PayLoad<string>> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.users.Where(x => !x.deleted).AsQueryable();

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(x => x.username.Contains(name) || x.fullname.Contains(name));

                var data = query.Select(x => new
                {
                    x.id,
                    x.fullname,
                    x.username,
                    x.email,
                    x.phone,
                    x.image,
                    x.quocgia,
                    x.role_id
                }).ToList();

                var pageList = new PageList<object>(data, page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> Login(LoginDTO loginDTO)
        {
            try
            {
                var checkData = _context.users.Include(r => r.roles)
                    .Where(x => (x.username == loginDTO.username || x.email == loginDTO.username) && !x.deleted)
                    .FirstOrDefault();

                if (checkData == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                // Hỗ trợ cả BCrypt (user mới) và SHA256 (user cũ)
                bool isValid = false;
                if (checkData.password != null && checkData.password.StartsWith("$2"))
                {
                    // BCrypt hash
                    isValid = EncryptionHelper.VerifyPassword(loginDTO.password, checkData.password);
                }
                else
                {
                    // SHA256 hash (user cũ) — verify rồi migrate sang BCrypt
                    var sha256Hash = EncryptionHelper.CreatePasswordHash(loginDTO.password, _jwt.Key);
                    isValid = checkData.password == sha256Hash;
                    if (isValid)
                    {
                        // Tự động migrate password sang BCrypt
                        checkData.password = EncryptionHelper.HashPassword(loginDTO.password);
                        _context.users.Update(checkData);
                        await _context.SaveChangesAsync();
                    }
                }

                if (!isValid)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var claims = new List<Claim>() { 
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(Status.IDAUTHENTICATION, checkData.id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, checkData.id.ToString())
                };

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    id = checkData.id,
                    username = checkData.username,
                    email = checkData.email,
                    fullname = checkData.fullname,
                    image = checkData.image,
                    role = checkData.roles == null ? "Chưa có role" : checkData.roles.name,
                    quocgia = checkData.quocgia,
                    phone = checkData.phone,
                    signature_name = checkData.signature_name,
                    signature_font = checkData.signature_font,
                    signature_size = checkData.signature_size,
                    token = genToken(claims)
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private string genToken(List<Claim> claims)
        {
            var security = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creadentian = new SigningCredentials(security, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    _jwt.Issuer,
                    _jwt.Issuer,
                    expires: DateTime.Now.AddMinutes(120),
                    claims: claims,
                    signingCredentials: creadentian
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<PayLoad<UserDTO>> Update(int id, UserDTO data)
        {
            throw new NotImplementedException();
        }

        public async Task<PayLoad<object>> uploadImageUser(ImageUserUpload data)
        {
            try
            {
                var user = _userNameLoginService.name();
                var checkAccount = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(user) && !x.deleted);
                if(checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                if (data.file != null && data.file.Any() && data.file.Count() > 0)
                {
                    var listImage = new List<image_user>();
                    foreach(var item in data.file)
                    {
                        var cloudResult = uploadCloud.CloudInaryIFromAccount(item, Status.IMAGEUSER + "_" + checkAccount.fullname, _cloud);
                        listImage.Add(new image_user
                        {
                            image = cloudResult.Link,
                            public_id = cloudResult.PublicId,
                            user = checkAccount,
                            user_id = checkAccount.id
                        });
                    }

                    _context.image_Users.AddRange(listImage);
                    await _context.SaveChangesAsync();
                }

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = data
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAlluploadImageUser()
        {
            try
            {
                int userId = int.Parse(_userNameLoginService.name());

                var data = _context.image_Users.Where(x => x.user_id == userId && !x.deleted)
                    .Select(x => new
                    {
                        x.id,
                        x.image,
                        x.isUsed,
                        x.cretoredat
                    }).ToList();

                var checkTotalLike = _context.likes.Count(x => x.user_id == userId && !x.deleted && x.isLiked == true);
                var checkComment = _context.comments.Count(x => x.user_id == userId && !x.deleted);
                var checkImageTotal = _context.image_Users.Count(x => x.user_id == userId && !x.deleted);
                var checkPostTotal = _context.posts.Count(x => x.user_id == userId && !x.deleted);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = data,
                    like = checkTotalLike,
                    comment = checkComment,
                    imageTotal = checkImageTotal,
                    postTotal = checkPostTotal
                }));
            }catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> ForgotPassword(ForgotPasswordDTO data)
        {
            try
            {
                var checkAccount = _context.users.FirstOrDefault(x => x.email == data.email && !x.deleted);
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                // Kiểm tra mật khẩu cũ (hỗ trợ cả BCrypt và SHA256)
                bool isOldPasswordValid = false;
                if (checkAccount.password != null && checkAccount.password.StartsWith("$2"))
                    isOldPasswordValid = EncryptionHelper.VerifyPassword(data.oldPassword, checkAccount.password);
                else
                    isOldPasswordValid = checkAccount.password == EncryptionHelper.CreatePasswordHash(data.oldPassword, _jwt.Key);

                if (!isOldPasswordValid)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.PASSWORDOLDFAILD));

                if (data.newPassword != data.confirmPassword)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.PASSWORDOLDFAILD));

                // Mật khẩu mới luôn dùng BCrypt
                checkAccount.password = EncryptionHelper.HashPassword(data.newPassword);
                checkAccount.updateat = DateTimeOffset.UtcNow;

                _context.users.Update(checkAccount);
                await _context.SaveChangesAsync();

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    message = Status.UPDATEPASSWORD,
                    email = checkAccount.email
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> UpdateProfile(UpdateProfileDTO data)
        {
            try
            {
                // Lấy user hiện tại từ JWT token
                var userId = _userNameLoginService.name();
                var checkAccount = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(userId) && !x.deleted);
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                // Cập nhật fullname nếu có
                if (!string.IsNullOrEmpty(data.fullname))
                    checkAccount.fullname = data.fullname;

                // Cập nhật image nếu có (upload lên Cloudinary)
                if (data.image != null)
                {
                    // Xóa ảnh cũ trên Cloudinary nếu có
                    if (!string.IsNullOrEmpty(checkAccount.publicid))
                        uploadCloud.DeleteImageItemCloud(checkAccount.publicid);

                    var cloudResult = uploadCloud.CloudInaryIFromAccount(data.image, Status.SOCIAL + "_" + checkAccount.email, _cloud);
                    checkAccount.image = cloudResult.Link;
                    checkAccount.publicid = cloudResult.PublicId;
                }

                // Cập nhật chữ ký nếu có
                if (!string.IsNullOrEmpty(data.signature_name))
                    checkAccount.signature_name = data.signature_name;

                if (!string.IsNullOrEmpty(data.signature_font))
                    checkAccount.signature_font = data.signature_font;

                if (data.signature_size.HasValue)
                    checkAccount.signature_size = data.signature_size;

                checkAccount.updateat = DateTimeOffset.UtcNow;

                _context.users.Update(checkAccount);
                await _context.SaveChangesAsync();

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    id = checkAccount.id,
                    fullname = checkAccount.fullname,
                    username = checkAccount.username,
                    email = checkAccount.email,
                    image = checkAccount.image,
                    quocgia = checkAccount.quocgia,
                    phone = checkAccount.phone,
                    signature_name = checkAccount.signature_name,
                    signature_font = checkAccount.signature_font,
                    signature_size = checkAccount.signature_size
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetProfile()
        {
            try
            {
                var userId = _userNameLoginService.name();
                var checkAccount = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(userId) && !x.deleted);
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    fullname = checkAccount.fullname,
                    email = checkAccount.email,
                    phone = checkAccount.phone,
                    image = checkAccount.image,
                    quocgia = checkAccount.quocgia,
                    signature_name = checkAccount.signature_name,
                    signature_font = checkAccount.signature_font,
                    signature_size = checkAccount.signature_size
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> DeleteImageUser(int imageId)
        {
            try
            {
                var userId = _userNameLoginService.name();
                var checkImage = _context.image_Users.FirstOrDefault(x => x.id == imageId && x.user_id == Convert.ToInt32(userId) && !x.deleted);
                if (checkImage == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                // Xóa ảnh trên Cloudinary
                if (!string.IsNullOrEmpty(checkImage.public_id))
                    uploadCloud.DeleteImageItemCloud(checkImage.public_id);

                // Soft delete các Post_Image liên quan
                var postImages = _context.Post_Images.Where(x => x.image_user_id == imageId && !x.deleted).ToList();
                foreach (var pi in postImages)
                {
                    pi.deleted = true;
                    pi.updateat = DateTimeOffset.UtcNow;
                }

                // Soft delete ảnh
                checkImage.deleted = true;
                checkImage.updateat = DateTimeOffset.UtcNow;

                _context.image_Users.Update(checkImage);
                await _context.SaveChangesAsync();

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    message = "Xóa ảnh thành công",
                    imageId = imageId
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
