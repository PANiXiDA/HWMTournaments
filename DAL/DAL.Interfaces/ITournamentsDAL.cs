using Common.ConvertParams;
using Common.SearchParams;
using DAL.Interfaces.Core;

namespace DAL.Interfaces;

public interface ITournamentsDAL : IBaseDAL<DbModels.Tournament, Entities.Tournament, int, TournamentsSearchParams, TournamentsConvertParams>
{
}