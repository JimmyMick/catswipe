using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;

namespace catswipe
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IOptions<InfoSec> _config;

        private readonly FileInfo _myImage;

        public Worker(ILogger<Worker> logger, IOptions<InfoSec> config) 
        {
            _config = config;
            _logger = logger;

            _myImage = new FileInfo(_config.Value.MyImage);

            _logger.LogInformation("Setting interval to {interval} milliseconds", _config.Value.Refresh);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking LockScreen Image: {time}", DateTimeOffset.Now);


                try{

                    var lockscreen = new FileInfo(_config.Value.Lockscreen);

                    if(_myImage.Length != lockscreen.Length){
                        _logger.LogInformation("Updating LockScreen Image: {time}", DateTimeOffset.Now);
                            File.Copy(_myImage.FullName, lockscreen.FullName, true);
                    }

                }
                catch(Exception e){
                    _logger.LogError("Unable to Update LockScreen Image: {error}", e.Message );
                }


                await Task.Delay(_config.Value.Refresh, stoppingToken);
            }
        }
    }
}
