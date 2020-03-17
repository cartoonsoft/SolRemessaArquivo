using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using wsRemessaArquivo.classes;

namespace wsRemessaArquivo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Remessa de Arquivo iniciado às: {time}", DateTimeOffset.Now);

                using (Emailconfig emailconfig = new Emailconfig())
                {
                    emailconfig.Host = _configuration.GetValue<string>("EmailSettings:Host");
                    emailconfig.Key = _configuration.GetValue<string>("EmailSettings:Hey");
                    if (Int32.TryParse(_configuration.GetValue<string>("EmailSettings:Port"), out int porta))
                    {
                        emailconfig.Port = porta;
                    } else {
                        emailconfig.Port = 587;
                    }
                    emailconfig.Sender = _configuration.GetValue<string>("EmailSettings:Sender");
                    emailconfig.User = _configuration.GetValue<string>("EmailSettings:User");

                    using (Email email = new Email(emailconfig))
                    {

                        using (RemessaArquivo remessa = new RemessaArquivo(_configuration, email))
                        {
                            remessa.ProcessarRemessa();
                        }
                    }
                }

                await Task.Delay(6000, stoppingToken);
            }
        }
    }
}
