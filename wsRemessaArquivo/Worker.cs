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
                using (Email email = new Email(_configuration))
                {
                    await email.EnviarEmail("Teste Remessa de Arquivo", "Início conversão de arquivo");
                    /*
                    using (RemessaArquivo remessa = new RemessaArquivo(_configuration, email))
                    {
                        _logger.LogInformation("Início Processando Remessa às: {time}", DateTimeOffset.Now);
                        remessa.ProcessarRemessa();
                        _logger.LogInformation("Fim Processando Remessa às: {time}", DateTimeOffset.Now);
                    }
                    */
                }

                await Task.Delay(6000, stoppingToken);
            }
        }

    }
}
