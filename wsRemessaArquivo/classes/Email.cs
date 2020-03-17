using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wsRemessaArquivo.Interfaces;

namespace wsRemessaArquivo.classes
{
    public class Emailconfig: IDisposable
    {
        /// <summary>
        /// constructor
        /// </summary>
        public Emailconfig()
        {

        }

        // Flag: Has Dispose already been called?
        private bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~Emailconfig()
        {
            Dispose(false);
        }

        public string Host { get; set; }
        public string Sender { get; set; }
        public string Key { get; set; }
        public string User { get; set; }
        public int Port { get; set; }
    }

    public class Email: IEmail
    {
        private readonly Emailconfig _emailconfig;

        public Email(Emailconfig emailconfig)
        {
            this._emailconfig = emailconfig;
        }

        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~Email()
        {
            Dispose(false);
        }

        private void Enviar_Email(List<string> listaDestinatarios, List<string> listaCC, string subject, string htmlBody)
        {
            try
            {
                using (MailMessage message = new MailMessage())
                {
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = _emailconfig.Host;
                        smtp.Port = _emailconfig.Port;
                        message.IsBodyHtml = true; //to make message body as html  
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Credentials = new NetworkCredential(_emailconfig.Sender, _emailconfig.Key);

                        message.From = new MailAddress(_emailconfig.Sender, "Portal de documentos");
                        foreach (var emailDestinatario in listaDestinatarios)
                        {
                            message.To.Add(new MailAddress(emailDestinatario));
                        }
                        foreach (var emailCC in listaCC)
                        {
                            message.To.Add(new MailAddress(emailCC));
                        }

                        message.Subject = "Test";
                        message.Body = htmlBody;

                        smtp.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task EnviarEmail(List<string> listaDestinatarios, List<string> listaCC, string subject, string htmlBody)
        {
            this.Enviar_Email(listaDestinatarios, listaCC, subject, htmlBody);
            await Task.Delay(3000);

        }


    }
}
