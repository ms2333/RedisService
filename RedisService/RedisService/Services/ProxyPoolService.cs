using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedisService.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RedisService
{
    public class ProxyPoolService: BackgroundService
    {
        private readonly ILogger<ProxyPoolService> _logger;
        private readonly ContainerBuilder _containerBuilder;
        private IContainer _container;
        private IDataService _IDataService;
        private ConcurrentDictionary<string,string> _curDic;

        public ProxyPoolService(ILogger<ProxyPoolService> logger)
        {
            _logger = logger;
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterType<DataService>().As<IDataService>().Named<IDataService>("DataService").SingleInstance();
            _curDic = new ConcurrentDictionary<string, string>();
        }

        //public override Task StopAsync(CancellationToken cancellationToken)
        //{
        //    Console.WriteLine(DateTime.Now + "任务结束");
        //    return Task.CompletedTask;
        //}

        protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(DateTime.Now + "    服务启动...");
            _container = _containerBuilder.Build();
            _logger.LogInformation(DateTime.Now + "    创建容器...");
            if (_container.IsRegisteredWithName<IDataService>("DataService"))
            {
                _IDataService = _container.ResolveNamed<IDataService>("DataService");
                _logger.LogInformation(DateTime.Now + "    获取数据服务实例...");
            }
            _logger.LogInformation(DateTime.Now + "    开始可用代理代理池检测...");
            //实现代码
            //1.心跳检测
            while (true)
            {
                await LiveCheck();
                Thread.Sleep(500);
            }
        }

        public async Task LiveCheck()
        {
            string ipStr = await _IDataService.get();
            if (ipStr == null)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                _logger.LogInformation(DateTime.Now + "    代理池为空");
                return;
            }
            if (await CheckProxyIpAsync(ipStr))
            {
                await _IDataService.saveAva(ipStr);//放入可用池，原池丢弃
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                _logger.LogInformation(DateTime.Now + "    代理池中的" + ipStr + "已失效");
            }
            //return Task.CompletedTask;
        }

        /// <summary>
        /// 验证代理ip
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<bool> CheckProxyIpAsync(string ipAddress)
        {
            int index = ipAddress.IndexOf(":");
            if (index == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                _logger.LogInformation("无效的地址： " + ipAddress);
                return true;
            }
            string proxyIp = ipAddress.Substring(0, index);
            int proxyPort = int.Parse(ipAddress.Substring(index + 1));
            var proxyHttpClientHandler = new HttpClientHandler
            {
                Proxy = new WebProxy(proxyIp, proxyPort),
                UseProxy = true,
            };
            using (var client = new HttpClient(proxyHttpClientHandler))
            {
                try
                {
                    client.Timeout = new TimeSpan(0,0,5);
                    var message = await client.GetAsync("http://cn.bing.com/");
                    if (message.IsSuccessStatusCode)
                    {
                        if (!_curDic.ContainsKey(ipAddress)) 
                        { 
                            _curDic.TryAdd<string, string>(ipAddress, "");
                            _logger.LogWarning("地址可用： " + ipAddress);
                        }
                        return true;
                    }
                        return false;
                }
                catch(Exception ex) 
                {
                    _logger.LogInformation(ipAddress + "_message:  " + ex.Message);
                    return false;
                }
                
            }
            
        }
    }
}
