using Common.SearchParams.Core;

namespace UI.Client.Services.Interfaces.Core;

public interface ICrudService<TDTO, TDTOId, TSearchParams, TConvertParams>
    where TDTO : class
    where TDTOId : struct
    where TSearchParams : class
    where TConvertParams : class
{
    Task<TDTO?> GetAsync(TDTOId id, TConvertParams? convertParams = null, CancellationToken cancellationToken = default);
    Task<SearchResult<TDTO>?> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null, CancellationToken cancellationToken = default);
    Task<TDTOId?> CreateAsync(TDTO dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(TDTOId id, TDTO dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(TDTOId id, CancellationToken cancellationToken = default);
}
