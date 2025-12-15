using AutoMapper;
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
        public PostService(DBContext context, IUserNameLoginService users, IMapper mapper)
        {
            _context = context;
            _user = users;
            _mapper = mapper;
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

        public Task<PayLoad<object>> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.posts.Where(x => !x.deleted).Select(x => new PostGetData
                {
                    id_post = x.id,
                    id_user = x.user_id,
                    fullname = x.user.fullname,
                    category_id = x.category_id,
                    category_name = x.category.name,
                    description = x.description,
                    created_at = x.cretoredat.Value,
                    images = x.post_Images != null && x.post_Images.Count > 0 ? x.post_Images.Where(x1 => x1.post_id == x.id).Select(x1 => x1.image).ToList() : new List<string?> { },
                    image_image = x.user.image,
                    isBackground = x.isBackground,
                    title = x.title,
                    totalComment = x.totalComment,
                    totalLine = x.totalLine
                }).OrderByDescending(x => x.created_at).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.title.Contains(name) || x.fullname.Contains(name) || x.category_name.Contains(name)).ToList();

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
    }
}
