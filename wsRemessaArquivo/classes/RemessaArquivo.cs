using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using wsRemessaArquivo.Interfaces;

namespace wsRemessaArquivo.classes
{
    public class RemessaArquivo: IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IEmail _email;

        public RemessaArquivo(IConfiguration configuration, IEmail email)
        {
            _configuration = configuration;
            _email = email;
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

        ~RemessaArquivo()
        {
            Dispose(false);
        }

        private void DeleteTempFile(string fileName)
        {
            string tempPath = _configuration.GetValue<string>("FilePathsSettings:CaminhoArqTemp");
            fileName = Path.Combine(tempPath, fileName);

            if (File.Exists(fileName)) 
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //enviar email
                }
            }

        }

        public void ProcessarRemessa()
        {
            DeleteTempFile("entrada.txt");
            DeleteTempFile("saida.txt");

            List<string> listaDest = new List<string>();
            List<string> listaCC = new List<string>();

            var listaDestTmp = _configuration.GetSection("EmailSettings:recipient_list").AsEnumerable();
            var listaCctmp = _configuration.GetSection("EmailSettings:cc_list").AsEnumerable();

            foreach (var item in listaDestTmp)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    listaDest.Add(item.Value);
                }
            }
            foreach (var item in listaCctmp)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    listaCC.Add(item.Value);
                }
            }

            string[] fileEntries = Directory.GetFiles(targetDirectory);

            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }



            /*
            string arqOrigem  = Path.Combine(pathRemessa, this._fileName);
            //string[] files = Directory.GetFiles(@"c:\MyDir\", "*.dat");

            if (!File.Exists(arqOrigem)) 
            {
                //todo: enviar email
                throw new FileLoadException("Erro ao abrir arquivo", arqOrigem);
            }
            
            string arqDestino = Path.GetFileNameWithoutExtension(this._fileName) +"_" + File.GetLastWriteTime(arqOrigem).ToString("yyyyMMdd")+ ".dat";

            //string arqOri = Path.Combine(pathRemessa, this._fileName);

            StreamReader sr = File.OpenText(arqOrigem);
            while (sr.Peek() != -1)
            {

                //Console.WriteLine(sr.ReadLine());
            }
            sr.Close();
            */

        }

    }
}
