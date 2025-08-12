using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using DTOs.Models;

namespace UI.Client.Services.Interfaces;

public interface IUsersService
{
    Task<UserDTO?> GetAsync(int id, UsersConvertParams? convertParams = null, CancellationToken cancellationToken = default);
    Task<SearchResult<UserDTO>?> GetAsync(UsersSearchParams searchParams, UsersConvertParams? convertParams = null, CancellationToken cancellationToken = default);
    Task<int?> CreateAsync(UserDTO dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UserDTO dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
