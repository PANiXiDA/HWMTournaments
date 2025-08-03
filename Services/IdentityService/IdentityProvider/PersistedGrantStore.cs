using Common.Constants;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;

using RedisClient.Interfaces;

namespace IdentityService.IdentityProvider;

public class PersistedGrantStore : IPersistedGrantStore
{
    private readonly IRedisCache _redisCache;

    public PersistedGrantStore(IRedisCache redisCache)
    {
        _redisCache = redisCache;
    }

    public async Task<PersistedGrant?> GetAsync(string key)
    {
        var redisKey = $"{RedisKeysConstants.PersistedGrantKeyPrefix}:{key}";
        var (found, value) = await _redisCache.TryGetAsync<PersistedGrant>(redisKey);
        if (!found || value is null) return null;

        return value;
    }

    public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
    {
        IEnumerable<PersistedGrant> persistedGrants;

        if (string.IsNullOrEmpty(filter.SubjectId))
        {
            var allPattern = $"{RedisKeysConstants.PersistedGrantKeyPrefix}:*";
            var all = await _redisCache.GetByPatternAsync<PersistedGrant>(allPattern);
            if (all.Count == 0) return Enumerable.Empty<PersistedGrant>();
            persistedGrants = all.Values;
        }
        else
        {
            var indexPattern = $"{RedisKeysConstants.PersistedGrantIndexKeyPrefix}:{filter.SubjectId}:*";
            var indexes = await _redisCache.GetByPatternAsync<string>(indexPattern);
            if (indexes.Count == 0) return Enumerable.Empty<PersistedGrant>();
            persistedGrants = await _redisCache.GetAsync<PersistedGrant>(indexes.Values.ToArray());
        }

        return persistedGrants.Where(grant => Filter(grant, filter));
    }

    public async Task StoreAsync(PersistedGrant persistedGrant)
    {
        var persistedGrantKey = $"{RedisKeysConstants.PersistedGrantKeyPrefix}:{persistedGrant.Key}";
        var expiredTime = persistedGrant.Expiration switch
        {
            null => (TimeSpan?)null,
            var dateTime when dateTime.Value <= DateTime.UtcNow => TimeSpan.Zero,
            var dateTime => dateTime.Value.ToUniversalTime() - DateTime.UtcNow
        };
        await _redisCache.SetAsync(persistedGrantKey, persistedGrant, expiredTime);

        if (!string.IsNullOrEmpty(persistedGrant.SubjectId))
        {
            var indexKey = $"{RedisKeysConstants.PersistedGrantIndexKeyPrefix}:{persistedGrant.SubjectId}:{persistedGrant.Key}";
            await _redisCache.SetAsync(indexKey, persistedGrantKey, expiredTime);
        }
    }

    public async Task RemoveAsync(string key)
    {
        var redisKey = $"{RedisKeysConstants.PersistedGrantKeyPrefix}:{key}";

        var (found, persistedGrant) = await _redisCache.TryGetAsync<PersistedGrant>(redisKey);
        await _redisCache.RemoveAsync(redisKey);

        if (found && persistedGrant is not null && !string.IsNullOrEmpty(persistedGrant.SubjectId))
        {
            var indexKey = $"{RedisKeysConstants.PersistedGrantIndexKeyPrefix}:{persistedGrant.SubjectId}:{persistedGrant.Key}";
            await _redisCache.RemoveAsync(indexKey);
        }
    }

    public async Task RemoveAllAsync(PersistedGrantFilter filter)
    {
        var indexPattern = $"{RedisKeysConstants.PersistedGrantIndexKeyPrefix}:{filter.SubjectId ?? string.Empty}:*";
        var indexes = await _redisCache.GetByPatternAsync<string>(indexPattern);

        if (indexes.Count == 0) return;
        var persistedGrants = await _redisCache.GetAsync<PersistedGrant>(indexes.Values.ToArray());

        persistedGrants = persistedGrants.Where(persistedGrant => Filter(persistedGrant, filter)).ToList();
        var grantKeysToDelete = new List<string>();
        var indexKeysToDelete = new List<string>();

        foreach (var persistedGrant in persistedGrants)
        {
            var grantKeyToDelete = $"{RedisKeysConstants.PersistedGrantKeyPrefix}:{persistedGrant.Key}";
            grantKeysToDelete.Add(grantKeyToDelete);
            if (!string.IsNullOrEmpty(persistedGrant.SubjectId))
            {
                var indexKeyToDelete = $"{RedisKeysConstants.PersistedGrantIndexKeyPrefix}:{persistedGrant.SubjectId}:{persistedGrant.Key}";
                indexKeysToDelete.Add(indexKeyToDelete);
            }
        }

        await _redisCache.RemoveAsync(grantKeysToDelete.ToArray());
        await _redisCache.RemoveAsync(indexKeysToDelete.ToArray());
    }

    private static bool Filter(PersistedGrant grant, PersistedGrantFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.SubjectId) &&
            !string.Equals(grant.SubjectId, filter.SubjectId, StringComparison.Ordinal))
        {
            return false;
        }
        if (!string.IsNullOrEmpty(filter.ClientId) &&
            !string.Equals(grant.ClientId, filter.ClientId, StringComparison.Ordinal))
        {
            return false;
        }
        if (!string.IsNullOrEmpty(filter.SessionId) &&
            !string.Equals(grant.SessionId, filter.SessionId, StringComparison.Ordinal))
        {
            return false;
        }
        if (!string.IsNullOrEmpty(filter.Type) &&
            !string.Equals(grant.Type, filter.Type, StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }
}
