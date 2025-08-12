using Common.ConvertParams;
using Common.SearchParams;

using DTOs.Models;

using UI.Client.Services.Interfaces.Core;

namespace UI.Client.Services.Interfaces;

public interface IUsersService : ICrudService<UserDTO, int, UsersSearchParams, UsersConvertParams>
{
}
