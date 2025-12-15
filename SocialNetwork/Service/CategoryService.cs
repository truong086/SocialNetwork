using AutoMapper;
using SocialNetwork.Common;
using SocialNetwork.Models;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly DBContext _context;
        private IMapper _mapper;
        private readonly IUserNameLoginService _userLogin;
        public CategoryService(DBContext context, IMapper mapper, IUserNameLoginService userLogin)
        {
            _context = context;
            _mapper = mapper;
            _userLogin = userLogin;
        }
        public async Task<PayLoad<CategoryDTO>> Add(CategoryDTO data)
        {
            try
            {
                var user = _userLogin.name();
                var checkAccount = _context.users.FirstOrDefault(x => x.id == int.Parse(user) && !x.deleted);
                var checkName = _context.categories.FirstOrDefault(x => x.name == data.name && !x.deleted);

                if (checkName != null || checkAccount == null)
                    return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<Category>(data);
                mapData.user = checkAccount;
                mapData.user_id = checkAccount.id;

                _context.categories.Add(mapData);
                await _context.SaveChangesAsync();

                return await Task.FromResult(PayLoad<CategoryDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<CategoryDTO>.CreatedFail(ex.Message));
            }
        }

        public Task<PayLoad<string>> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var data = _context.categories.Where(x => !x.deleted).
                    Select(x => new
                    {
                        x.id,
                        x.name,
                        x.description,
                        x.deleted,
                        x.user.username
                    }).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name) && !x.deleted).ToList();

                var pageList = new PageList<object>(data, page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
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

        public Task<PayLoad<CategoryDTO>> Update(int id, CategoryDTO data)
        {
            throw new NotImplementedException();
        }
    }
}
