using Common.ConvertParams;
using Common.SearchParams;

using DAL.Interfaces.Core;

namespace DAL.Interfaces;

public interface IUsersDAL : IBaseDAL<DbModels.User, Entities.User, int, UsersSearchParams, UsersConvertParams>
{
}