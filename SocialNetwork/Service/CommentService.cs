using SocialNetwork.Common;
using SocialNetwork.Models;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Service
{
    public class CommentService : ICommentService
    {
        private readonly DBContext _context;
        private readonly IUserNameLoginService _userNameLoginService;
        public CommentService(DBContext context, IUserNameLoginService userNameLoginService)
        {
            _context = context;
            _userNameLoginService = userNameLoginService;

        }
        public async Task<PayLoad<object>> AddComment(CommentDTO data)
        {
            try
            {
                int user = Convert.ToInt32(_userNameLoginService.name());
                
                var checkAccount = _context.users.FirstOrDefault(x => x.id == user && !x.deleted);
                var checkPost = _context.posts.FirstOrDefault(x => x.id == data.post_id && !x.deleted);
                if (checkAccount == null || checkPost == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                _context.comments.Add(new Comment
                {
                    post_id = checkPost.id,
                    post = checkPost,
                    user = checkAccount,
                    user_id = checkAccount.id,
                    comment = data.description
                });

                if(await _context.SaveChangesAsync() > 0)
                {
                    int totalCoutCommentPost = _context.comments.Where(x => x.post_id == checkPost.id && !x.deleted).Count();
                    checkPost.totalComment = totalCoutCommentPost;
                    _context.posts.Update(checkPost);
                    _context.SaveChanges();

                    var dataNew = _context.comments.OrderByDescending(x => x.id).FirstOrDefault();
                    return await Task.FromResult(PayLoad<object>.Successfully(new commentItem
                    {
                        id = dataNew.id,
                        id_post = checkPost.id,
                        image_user = checkAccount.image,
                        text = data.description,
                        user = checkAccount.fullname
                    }));
                }

                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATAERROR));
            }catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
