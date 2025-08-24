using book_shop.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace book_shop.Services.Implementations
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis?.GetDatabase() ?? throw new ArgumentNullException(nameof(redis));
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return default;

            var value = await _db.StringGetAsync(key);
            return value.HasValue
                ? JsonSerializer.Deserialize<T>(value!)
                : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                await _db.KeyDeleteAsync(key);
            }
        }
    }
}
