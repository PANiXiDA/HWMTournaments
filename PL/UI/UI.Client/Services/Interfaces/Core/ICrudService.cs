using Common.SearchParams.Core;

namespace UI.Client.Services.Interfaces.Core;

public interface ICrudService<TEntity, TEntityId, TSearchParams, TConvertParams>
    where TEntity : class
    where TEntityId : struct
    where TSearchParams : class
    where TConvertParams : class
{
    Task<TEntity?> GetAsync(TEntityId id, TConvertParams? convertParams = null, CancellationToken cancellationToken = default);
    Task<SearchResult<TEntity>?> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null, CancellationToken cancellationToken = default);
    Task<TEntityId?> CreateAsync(TEntity dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntityId id, TEntity dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntityId id, CancellationToken cancellationToken = default);
}
