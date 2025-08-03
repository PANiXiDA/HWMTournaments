using BL.Interfaces;

using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using DAL.Interfaces;

using User = Entities.User;

namespace BL.Implementations;

public sealed class UsersBL : IUsersBL
{
    private readonly IUsersDAL _usersDAL;

    public UsersBL(IUsersDAL usersDAL)
    {
        _usersDAL = usersDAL;
    }

    public Task<User> GetAsync(int id, UsersConvertParams? convertParams = null)
    {
        return _usersDAL.GetAsync(id, convertParams);
    }

    public Task<SearchResult<User>> GetAsync(UsersSearchParams searchParams, UsersConvertParams? convertParams = null)
    {
        return _usersDAL.GetAsync(searchParams, convertParams);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _usersDAL.ExistsAsync(id);
    }

    public Task<bool> ExistsAsync(UsersSearchParams searchParams)
    {
        return _usersDAL.ExistsAsync(searchParams);
    }

    public async Task<int> AddOrUpdateAsync(User entity)
    {
        entity.Id = await _usersDAL.AddOrUpdateAsync(entity);
        return entity.Id;
    }

    public Task<IList<int>> AddOrUpdateAsync(IList<User> entities)
    {
        return _usersDAL.AddOrUpdateAsync(entities);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _usersDAL.DeleteAsync(id);
    }

    public Task<bool> DeleteAsync(List<int> ids)
    {
        return _usersDAL.DeleteAsync(db => ids.Contains(db.Id));
    }

    public async Task<int> RegistrationAsync(User entity)
    {
        await AddOrUpdateAsync(entity);

        return entity.Id;
    }
}

