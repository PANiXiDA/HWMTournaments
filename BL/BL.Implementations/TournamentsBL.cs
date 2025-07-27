using BL.Interfaces;
using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;
using DAL.Interfaces;
using Entities;

public sealed class TournamentsBL : ITournamentsBL
{
    private readonly ITournamentsDAL _tournamentsDAL;

    public TournamentsBL(ITournamentsDAL tournamentsDAL)
    {
        _tournamentsDAL = tournamentsDAL;
    }

    public Task<Tournament> GetAsync(int id, TournamentsConvertParams? convertParams = null)
    {
        return _tournamentsDAL.GetAsync(id, convertParams);
    }

    public Task<SearchResult<Tournament>> GetAsync(TournamentsSearchParams searchParams, TournamentsConvertParams? convertParams = null)
    {
        return _tournamentsDAL.GetAsync(searchParams, convertParams);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _tournamentsDAL.ExistsAsync(id);
    }

    public Task<bool> ExistsAsync(TournamentsSearchParams searchParams)
    {
        return _tournamentsDAL.ExistsAsync(searchParams);
    }

    public async Task<int> AddOrUpdateAsync(Tournament entity)
    {
        entity.Id = await _tournamentsDAL.AddOrUpdateAsync(entity);
        return entity.Id;
    }

    public async Task<IList<int>> AddOrUpdateAsync(IList<Tournament> entities)
    {
        return await _tournamentsDAL.AddOrUpdateAsync(entities);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _tournamentsDAL.DeleteAsync(id);
    }

    public Task<bool> DeleteAsync(List<int> ids)
    {
        return _tournamentsDAL.DeleteAsync(db => ids.Contains(db.Id));
    }
}