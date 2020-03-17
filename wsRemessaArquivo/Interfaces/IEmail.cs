using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace wsRemessaArquivo.Interfaces
{
    public interface IEmail : IDisposable
    {
        Task EnviarEmail(List<string> listaDestinatarios, List<string> listaCC, string subject, string htmlBody);
    }
}
