using AutoMapper;
using SocialNetwork.Common;
using SocialNetwork.Models;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Service
{
    public class RoleService : IRoleService
    {
        private readonly DBContext _context;
        private IMapper _mapper;
        public RoleService(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PayLoad<roleDTO>> Add(roleDTO data)
        {
            try
            {
                var checkName = _context.roles.FirstOrDefault(x => x.name == data.name && !x.deleted);
                if (checkName != null)
                    return await Task.FromResult(PayLoad<roleDTO>.CreatedFail(Status.DATATONTAI));

                _context.roles.Add(_mapper.Map<role>(data));
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<roleDTO>.Successfully(data));
            }catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<roleDTO>.CreatedFail(ex.Message));
            }
        }

        public Task<PayLoad<string>> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 10, int pageSize = 20)
        {
            try
            {
                var data = _context.roles.Where(x => !x.deleted).ToList();
                
                if(!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.name.Contains(name) && !x.deleted).ToList();

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

        public Task<PayLoad<object>> FindOne(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PayLoad<roleDTO>> Update(int id, roleDTO data)
        {
            throw new NotImplementedException();
        }
    }
}
