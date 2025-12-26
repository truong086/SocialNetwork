using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialNetwork.Clouds;
using SocialNetwork.Common;
using SocialNetwork.Models;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Service
{
    public class PostService : IPostService
    {
        private readonly IUserNameLoginService _user;
        private readonly DBContext _context;
        private IMapper _mapper;
        private Cloud _cloud;
        private readonly IUserService _userService;
        public PostService(DBContext context, IUserNameLoginService users, IMapper mapper, IOptions<Cloud> cloud, IUserService userService)
        {
            _context = context;
            _user = users;
            _mapper = mapper;
            _cloud = cloud.Value;
            _userService = userService;
        }
        public async Task<PayLoad<PostDTO>> Add(PostDTO postDTO)
        {
            try
            {
                var user = _user.name();
                var checkAccount = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(user) && !x.deleted);
                var checkCategory = _context.categories.FirstOrDefault(x => x.id == postDTO.category_id && !x.deleted);

                if (checkAccount == null || checkCategory == null)
                    return await Task.FromResult(PayLoad<PostDTO>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<Post>(postDTO);
                mapData.category = checkCategory;
                mapData.user = checkAccount;
                mapData.category_id = checkCategory.id;
                mapData.user_id = checkAccount.id;

                _context.posts.Add(mapData);
                await _context.SaveChangesAsync();

                if(postDTO.images != null && postDTO.images.Count > 0)
                {
                    var dataNew = _context.posts.OrderByDescending(x => x.cretoredat).FirstOrDefault();
                    if(dataNew != null)
                    {
                        // Nếu như dữ liệu trong List là 1 object thì ép sang object đó, ở đây đang là ép sang object "user"
                        //List<int> listIds = postDTO
                        //    .Cast<User>()
                        //    .Select(x => x.Id)
                        //    .ToList();

                        List<int?> list = postDTO.images.Select(x => x.id).ToList();
                        var dataImages = _context.image_Users.Where(x => list.Contains(x.id) && !x.deleted).ToList();
                        List<Post_Image> listImage = new List<Post_Image>();
                        foreach (var item in postDTO.images)
                        {
                            if (list.Contains(item.id))
                            {
                                var checkDataImage = dataImages.FirstOrDefault(x => x.id == item.id);
                                if (checkDataImage != null)
                                    listImage.Add(new Post_Image
                                    {
                                        image = item.image,
                                        public_id = item.publicId,
                                        image_users = checkDataImage,
                                        image_user_id = checkDataImage.id,
                                        post = dataNew,
                                        post_id = dataNew.id,
                                        user = checkAccount,
                                        user_id = checkAccount.id
                                    });
                            }
                            
                        }

                        if(listImage.Count > 0)
                        {
                            _context.Post_Images.AddRange(listImage);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                return await Task.FromResult(PayLoad<PostDTO>.Successfully(postDTO));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<PostDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<PostEditImageDTO>> AddEditImage(PostEditImageDTO postDTO)
        {
            try
            {
                var user = _user.name();
                if (postDTO.images == null)
                    return await Task.FromResult(PayLoad<PostEditImageDTO>.CreatedFail(Status.DATANULL));
                var checkCategory = _context.categories.FirstOrDefault(x => x.id == postDTO.category_id && !x.deleted);
                var checkUser = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(user) && !x.deleted);

                if(checkCategory == null || checkUser == null)
                    return await Task.FromResult(PayLoad<PostEditImageDTO>.CreatedFail(Status.DATANULL));

                uploadCloud.CloudInaryIFromAccount(postDTO.images, Status.IMAGEUSER + "_" + checkUser.username, _cloud);
                _context.image_Users.Add(new image_user
                {
                    image = uploadCloud.Link,
                    public_id = uploadCloud.publicId,
                    user = checkUser,
                    user_id = checkUser.id
                });

                if(_context.SaveChanges() > 0)
                {
                    var dataNewImageUser = _context.image_Users.OrderByDescending(x => x.id).FirstOrDefault();
                    var listImage = new List<imageCloud>()
                    {
                        new imageCloud
                        {
                            id = dataNewImageUser.id, 
                            image = dataNewImageUser.image,
                            publicId = dataNewImageUser.public_id
                        }
                    };
                    
                    var addPostData = await Add(new PostDTO
                    {
                        images = listImage,
                        category_id = checkCategory.id,
                        description = "",
                        title = "",
                        isBackground = true,
                    });

                    if (!addPostData.Success)
                        return await Task.FromResult(PayLoad<PostEditImageDTO>.CreatedFail(Status.DATANULL));
                    return await Task.FromResult(PayLoad<PostEditImageDTO>.Successfully(postDTO));
                }
                return await Task.FromResult(PayLoad<PostEditImageDTO>.CreatedFail(Status.DATANULL));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<PostEditImageDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> addImageTestCloud(IFormFile? data)
        {
            try
            {
                if (data == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                uploadCloud.CloudInaryIFromAccount(data, "test_Cloud_Data", _cloud);
                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    url = uploadCloud.Link,
                    publicId = uploadCloud.publicId
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public Task<PayLoad<object>> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PayLoad<object>> FindAll(string? name, int? category = 0, int page = 1, int pageSize = 20)
        {
            try
            {
                int userId = int.Parse(_user.name());

                var data = _context.posts
                    .AsNoTracking()
                    .Where(x => !x.deleted).Select(x => new PostGetData
                {
                    id = x.id,
                    id_user = x.user_id,
                    name = x.user.fullname,
                    category_id = x.category_id,
                    category_name = x.category.name,
                    content = x.description,
                    time = x.cretoredat.Value,
                    images = x.post_Images != null && x.post_Images.Count > 0 ? x.post_Images.Where(x1 => x1.post_id == x.id).Select(x1 => x1.image).ToList() : new List<string?> { },
                    avatar = x.user.image,
                    isBackground = x.isBackground,
                    title = x.title,
                    totalComment = x.totalComment,
                    likes = x.totalLine,
                        //isUserLike = x.likes
                        //                .Where(xl => xl.user_id == int.Parse(user) 
                        //                && !x.deleted && xl.isLiked == true 
                        //                && xl.post_id == x.id)
                        //                .FirstOrDefault() == null ? false : true

                        // ✅ tối ưu check like
                        isUserLike = x.likes.Any(l => l.user_id == userId && l.isLiked == true && !x.deleted)
                    }).OrderByDescending(x => x.time);

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name) || x.name.Contains(name) || x.category_name.Contains(name)).OrderByDescending(x => x.time);

                if (category != null && category != 0 && category.HasValue)
                    data = data.Where(x => x.category_id == category).OrderByDescending(x => x.time);

                data.ToList(); // Làm tất cả mọi thứ xong mới được để "ToList()" vào, vì để quá sớm sẽ làm chậm

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

        public Task<PayLoad<object>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<PostDTO>> Update(int id, PostDTO postDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<PayLoad<object>> FindAllPostByUser(string? name, int? category, int page = 1, int pageSize = 20)
        {
            try
            {
                var user = _user.name();

                var data = _context.posts.Where(x => !x.deleted && x.user_id == int.Parse(user)).Select(x => new PostGetData
                {
                    id = x.id,
                    id_user = x.user_id,
                    name = x.user.fullname,
                    category_id = x.category_id,
                    category_name = x.category.name,
                    content = x.description,
                    time = x.cretoredat.Value,
                    images = x.post_Images != null && x.post_Images.Count > 0 ? x.post_Images.Where(x1 => x1.post_id == x.id).Select(x1 => x1.image).ToList() : new List<string?> { },
                    avatar = x.user.image,
                    isBackground = x.isBackground,
                    title = x.title,
                    totalComment = x.totalComment,
                    likes = x.totalLine,
                    isUserLike = x.likes.Any(xl => xl.user_id == int.Parse(user) && !x.deleted && xl.isLiked == true)
                }).OrderByDescending(x => x.time);

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name) || x.name.Contains(name) || x.category_name.Contains(name)).OrderByDescending(x => x.time);

                if (category != null && category != 0)
                    data = data.Where(x => x.category_id == category).OrderByDescending(x => x.time);

                data.ToList(); // Làm tất cả mọi thứ xong mới được để "ToList()" vào, vì để quá sớm sẽ làm chậm
                var pageList = new PageList<object>(data, page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> AddLike(int post_id)
        {
            try
            {
                int user = int.Parse(_user.name());

                var checkPost = _context.posts.FirstOrDefault(x => x.id == post_id && !x.deleted);
                var checkUser = _context.users.FirstOrDefault(x => x.id == user && !x.deleted);

                if (checkPost == null || checkUser == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var checkLikePost = _context.likes.FirstOrDefault(x => x.post_id == post_id && !x.deleted && x.user_id == user);
                if(checkLikePost == null)
                {
                    _context.likes.Add(new Like
                    {
                        post = checkPost,
                        post_id = checkPost.id,
                        user = checkUser,
                        user_id = checkUser.id,
                        isLiked = true
                    });

                    if(await _context.SaveChangesAsync() > 0)
                    {
                        var dataNew = totalCountLike(post_id);
                        checkPost.totalLine = dataNew;
                        _context.posts.Update(checkPost);
                        _context.SaveChanges();
                        return await Task.FromResult(PayLoad<object>.Successfully(new
                        {
                            postId = post_id,
                            totalLike = dataNew
                        }));
                    }
                }else if(checkLikePost != null)
                {
                    if(checkLikePost.isLiked == true) checkLikePost.isLiked = false;
                    else checkLikePost.isLiked = true;

                    _context.likes.Update(checkLikePost);
                    if(await _context.SaveChangesAsync() > 0)
                    {
                        var dataUpdate = totalCountLike(post_id);
                        checkPost.totalLine = dataUpdate;
                        _context.posts.Update(checkPost);
                        _context.SaveChanges();
                        return await Task.FromResult(PayLoad<object>.Successfully(new
                        {
                            postId = post_id,
                            totalLike = dataUpdate
                        }));
                    }
                }

                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private int totalCountLike(int post_id)
        {
            var dataNew = _context.likes.Where(x => x.post_id == post_id && !x.deleted && x.isLiked == true).Count();

            return dataNew;
        }
    }
}
