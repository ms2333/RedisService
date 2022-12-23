using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange;
using StackExchange.Redis.Extensions.Core.Implementations;

namespace RedisService.Services
{
    public class DataService : IDataService
    {
        private static readonly ConfigurationOptions ConfigurationOptions = ConfigurationOptions.Parse("");
        private static ConnectionMultiplexer _redisConnect;

        public DataService()
        {
            if (_redisConnect == null)
            {
                ConfigurationOptions.AbortOnConnectFail = false;
                _redisConnect = ConnectionMultiplexer.Connect(ConfigurationOptions);
            }
        }

        public async Task<string> get()
        {
            var db = _redisConnect.GetDatabase(); 
            string str = await db.ListLeftPopAsync("ipPool");
            return str;  
        }

        public async Task<string> getAva()
        {
            var db = _redisConnect.GetDatabase(); 
            string str = await db.ListLeftPopAsync("ipPoolAva");
            return str; 
        }

        public async Task<bool> save(string str)
        {
            var db = _redisConnect.GetDatabase();
            RedisValue[] ipAddrs = await db.ListRangeAsync("ipPool");
            if (!ipAddrs.Any(x => x.ToString() == str)) await db.ListRightPushAsync("ipPool", str);
            return true;
        }

        public async Task<bool> saveAva(string str)
        {
            var db = _redisConnect.GetDatabase();
            RedisValue[] ipAddrsAva = await db.ListRangeAsync("ipPoolAva");
            if (!ipAddrsAva.Any(x => x.ToString() == str)) await db.ListRightPushAsync("ipPoolAva", str);
            return true;
        }
    }
}
