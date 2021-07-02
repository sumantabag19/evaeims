using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace EVA.EIMS.Security.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Redis")]
    public class RedisController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public RedisController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        [HttpPost]
        public async Task<string> SetRedisTokenAsync([FromBody]string token)
        {
            string key = Guid.NewGuid().ToString();
            var data = Encoding.UTF8.GetBytes(token);
            await _distributedCache.SetAsync(key, data);
            return key;
        }
        [HttpPost("SetRedisKeyValueAsync")]
        public async Task<string> SetRedisKeyValueAsync(string key, [FromBody] string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            await _distributedCache.SetAsync(key, data);
            return key;
        }
        [HttpGet]
        public async Task<string> GetRedisTokenAsync(string key)
        {
            var cachedData = await _distributedCache.GetAsync(key);
            if (cachedData == null)
                return "No Such Key Found";
            var cachedMessage = Encoding.UTF8.GetString(cachedData);
            return cachedMessage;
        }

        [HttpDelete]
        public void RemoveRedisTokenAsync(string key)
        {
            _distributedCache.Remove(key);

        }
    }
}