using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisService
{
    internal interface IDataService
    {
        Task<bool> save(string str);
        Task<bool> saveAva(string str);
        Task<string> get();
        Task<string> getAva();



    }
}
