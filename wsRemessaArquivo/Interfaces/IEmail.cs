using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace wsRemessaArquivo.Interfaces
{
    public interface IEmail : IDisposable
    {
        string Host { get; set; }
        string Sender { get; set; }
        string Key { get; set; }
        string User { get; set; }
        int Port { get; set; }

        List<string> ListaDestinatarios { get; set; }
        List<string> ListaCC { get; set; }

        Task EnviarEmail( string subject, string htmlBody);
    }
}
