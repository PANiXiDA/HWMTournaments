using BL.Interfaces;

using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using DAL.Interfaces;

using Gen.IdentityService.ApplicationUserService;

using static Common.Constants.IdentityServiceConstants;

using User = Entities.User;

namespace BL.Implementations;

public sealed class UsersBL : IUsersBL
{
    private readonly ApplicationUserService.ApplicationUserServiceClient _applicationUserServiceClient;

    private readonly IUsersDAL _usersDAL;

    public UsersBL(
        ApplicationUserService.ApplicationUserServiceClient applicationUserServiceClient,
        IUsersDAL usersDAL)
    {
        _applicationUserServiceClient = applicationUserServiceClient;
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
        if (entity.Id == 0)
        {
            await CreateAsync(entity);
        }
        else
        {
            await UpdateAsync(entity);
        }

        return entity.Id;
    }

    public async Task<IList<int>> AddOrUpdateAsync(IList<User> entities)
    {
        foreach (var entity in entities)
        {
            await AddOrUpdateAsync(entity);
        }

        return entities.Select(item => item.Id).ToList();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        await _applicationUserServiceClient.DeleteAsync(new DeleteApplicationUserRequest { Id = entity.ApplicationUserId });
        return await _usersDAL.DeleteAsync(id);
    }

    public async Task<bool> DeleteAsync(List<int> ids)
    {
        foreach (var id in ids)
        {
            await DeleteAsync(id);
        }

        return true;
    }

    public async Task<int> RegistrationAsync(User entity)
    {
        await AddOrUpdateAsync(entity);
        return entity.Id;
    }

    private async Task CreateAsync(User entity)
    {
        var grpcResponse = await _applicationUserServiceClient.CreateAsync(entity.ApplicationUser);
        entity.ApplicationUserId = grpcResponse.Id;

        entity.Id = await _usersDAL.AddOrUpdateAsync(entity);

        await _applicationUserServiceClient.AddClaimAsync(
        new AddClaimRequest
        {
            ApplicationUserId = entity.ApplicationUserId,
            Type = CustomJwtClaimTypes.UserId,
            Value = entity.Id.ToString()
        });
    }

    private async Task UpdateAsync(User entity)
    {
        await _applicationUserServiceClient.UpdateAsync(entity.ApplicationUser);
        await _usersDAL.AddOrUpdateAsync(entity);
    }
}

