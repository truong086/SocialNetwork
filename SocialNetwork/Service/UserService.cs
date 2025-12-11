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
                var mapData = _mapper.Map<User>(userDTO);
                mapData.password = EncryptionHelper.CreatePasswordHash(mapData.password, _jwt.Key);
                
                mapData.roles = roleData;
                mapData.role_id = roleData == null ? null : roleData.id;
                if (userDTO.image != null)
                {
                    uploadCloud.CloudInaryIFromAccount(userDTO.image, Status.SOCIAL + "_" + mapData.email, _cloud);
                    mapData.image = uploadCloud.Link;
                    mapData.publicid = uploadCloud.publicId;
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
            if(id != null || id != 0)
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
                var data = _context.users.Where(x => !x.deleted).ToList();
                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.username.Contains(name) || x.fullname.Contains(name)).ToList();

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
                loginDTO.password = EncryptionHelper.CreatePasswordHash(loginDTO.password, _jwt.Key);
                var checkData = _context.users.Include(r => r.roles).Where(x => (x.username == loginDTO.username || x.email == loginDTO.username) && x.password == loginDTO.password && !x.deleted).FirstOrDefault();

                if (checkData == null)
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
                    role = checkData.roles.name,
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
                    expires: DateTime.Now.AddMinutes(12000),
                    claims: claims,
                    signingCredentials: creadentian
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<PayLoad<UserDTO>> Update(int id, UserDTO data)
        {
            throw new NotImplementedException();
        }

        public async Task<PayLoad<string>> uploadImageUser(ImageUserUpload data)
        {
            try
            {
                var user = _userNameLoginService.name();
                var checkAccount = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(user) && !x.deleted);
                if(checkAccount == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                if (data.file != null && data.file.Any() && data.file.Count() > 0)
                {
                    var listImage = new List<image_user>();
                    foreach(var item in data.file)
                    {
                        uploadCloud.CloudInaryIFromAccount(item, Status.IMAGEUSER + "_" + checkAccount.username, _cloud);
                        listImage.Add(new image_user
                        {
                            image = uploadCloud.Link,
                            public_id = uploadCloud.publicId,
                            user = checkAccount,
                            user_id = checkAccount.id
                        });
                    }

                    _context.image_Users.AddRange(listImage);
                    await _context.SaveChangesAsync();
                }

                return await Task.FromResult(PayLoad<string>.Successfully("Upload Success"));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAlluploadImageUser()
        {
            try
            {
                var data = _context.image_Users.ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = data
                }));
            }catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
