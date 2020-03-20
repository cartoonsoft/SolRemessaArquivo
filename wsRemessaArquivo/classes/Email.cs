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

    public class Email: IEmail
    {
        private readonly IConfiguration _configuration;
        private List<string> listaDestinatarios { get; set; }
        private List<string> listaCC { get; set; }

        public Email(IConfiguration configuration = null)
        {
            if (configuration != null)
            {
                this._configuration = configuration;
                this.Host = _configuration.GetValue<string>("EmailSettings:Host");
                this.Key = _configuration.GetValue<string>("EmailSettings:Key");
                if (Int32.TryParse(_configuration.GetValue<string>("EmailSettings:Port"), out int porta))
                {
                    this.Port = porta;
                }
                else
                {
                    this.Port = 587;
                }

                this.Sender = _configuration.GetValue<string>("EmailSettings:Sender");
                this.User = _configuration.GetValue<string>("EmailSettings:User");

                this.listaDestinatarios = this.LerListaEmailsAppSetings("EmailSettings:recipient_list");
                this.listaCC = this.LerListaEmailsAppSetings("EmailSettings:cc_list");
            }
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

        #region Private Prodedures, functions
        private List<string> LerListaEmailsAppSetings(string tipoLista)
        {
            List<string> lista = new List<string>();
            var listaDestTmp = _configuration.GetSection(tipoLista).AsEnumerable();

            foreach (var item in listaDestTmp)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    lista.Add(item.Value);
                }
            }

            return lista;
        }

        private void Enviar_Email(List<string> listaDestinatarios, List<string> listaCC, string subject, string htmlBody)
        {
            try
            {
                using (MailMessage message = new MailMessage())
                {
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = this.Host;
                        smtp.Port = this.Port;
                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(this.User, this.Key);
                        //smtp.EnableSsl = true;

                        message.IsBodyHtml = true; //to make message body as html  
                        message.From = new MailAddress(this.Sender, "Portal de documentos");
                        foreach (var emailDestinatario in listaDestinatarios)
                        {
                            message.To.Add(new MailAddress(emailDestinatario));
                        }
                        foreach (var emailCC in listaCC)
                        {
                            message.To.Add(new MailAddress(emailCC));
                        }
                        message.Subject = subject;
                        message.Body = htmlBody;

                        try
                        {
                            smtp.Send(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("smtp.Send >> " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        public string Host { get; set; }
        public string Sender { get; set; }
        public string Key { get; set; }
        public string User { get; set; }
        public int Port { get; set; }

        public List<string> ListaDestinatarios {
            get { return this.listaDestinatarios; }
            set { listaDestinatarios = value; }
        }

        public List<string> ListaCC {
            get { return this.listaCC; }
            set { listaCC = value; }
        }

        public async Task EnviarEmail(string subject, string htmlBody)
        {
            this.Enviar_Email(this.listaDestinatarios, this.listaCC, subject, htmlBody);
            await Task.Delay(3000);
        }

    }
}
