using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisService.Services
{
    //数据库代码自行实现
    internal class DBservice : IDataService
    {
        public Task<string> get()
        {
            throw new NotImplementedException();
        }

        public Task<string> getAva()
        {
            throw new NotImplementedException();
        }

        public Task<bool> save(string str)
        {
            throw new NotImplementedException();
        }

        public Task<bool> saveAva(string str)
        {
            throw new NotImplementedException();
        }
    }
}
