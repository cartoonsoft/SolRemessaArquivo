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

        private void DeleteFile(string fileName)
        {
            if (File.Exists(fileName)) 
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("DeleteFile >> {0}", ex.Message));
                    //enviar email
                }
            }
        }

        private bool CopiarArquivo(string arquivoOrigem, string arquivoDestino)
        {
            bool resp = false;

            if (File.Exists(arquivoOrigem))
            {
                try
                {
                    File.Copy(arquivoOrigem, arquivoDestino);
                    resp = true;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Erro ao copiar arquivo da Rede, File.Copy({0}, {1}) >> {2}", arquivoOrigem, arquivoDestino, ex.Message);
                    Console.WriteLine(msg);

                    this._email.EnviarEmail(
                        "Erro ao copiar arquivo na Rede",
                        string.Format(
                            "Arquivo não copiado de {0} para {1}, exeção gerada: {2}",
                            arquivoOrigem, arquivoDestino, ex.Message
                        )
                    );
                }
            } else {
                string msg = string.Format("Arquivo origem não existe, File.Copy({0}, {1})", arquivoOrigem, arquivoDestino);
                Console.WriteLine(msg);

                this._email.EnviarEmail(
                    "Arquivo de origem não encontrado na Rede",
                    string.Format(
                        "Arquivo de origem não encontrado na tentativa de copiar de {0} para {1}",
                        arquivoOrigem, arquivoDestino
                    )
                );
            }

            return resp;
        }

        public void ProcessarRemessa()
        {
            string caminhoArqOrigemRemessa = this._configuration.GetValue<string>("FilePathsSettings:CaminhoArqOrigemRemessa");
            string caminhoArqRemessa = this._configuration.GetValue<string>("FilePathsSettings:CaminhoArqRemessa");
            string caminhoDatR = this._configuration.GetValue<string>("FilePathsSettings:CaminhoDatR");
            string caminhoArqTemp = this._configuration.GetValue<string>("FilePathsSettings:CaminhoArqTemp");

            DeleteFile(Path.Combine(caminhoArqTemp, "entrada.txt"));
            DeleteFile(Path.Combine(caminhoArqTemp, "saida.txt"));

            string[] fileEntries = Directory.GetFiles(caminhoArqOrigemRemessa, "PORTAGCA.DAT*");

            foreach (string fileName in fileEntries)
            {
                if (File.Exists(fileName))
                {
                    string arqDest = "PORTAGCA_" + File.GetCreationTime(fileName).ToString("yyyyMMdd") +".DAT";

                    if (this.CopiarArquivo(fileName, Path.Combine(caminhoArqTemp, "entrada.txt")))
                    {
                        //ExeRun("C:\comp\COBOL34\conv.bat c:\tmp\entrada.txt c:\tmp\saida.txt", exeActive, exeWait)
                    }

                    if (File.Exists(Path.Combine(caminhoArqTemp, "saida.txt")))
                    {
                        if (!File.Exists(Path.Combine(caminhoArqRemessa, arqDest)))
                        {
                            if (this.CopiarArquivo(Path.Combine(caminhoArqTemp, "saida.txt"), Path.Combine(caminhoArqRemessa, arqDest))) 
                            {
                                this.CopiarArquivo(Path.Combine(caminhoArqRemessa, arqDest), Path.Combine(caminhoDatR, arqDest));
                            }
                        } else {
                            this._email.EnviarEmail(
                                "Arquivo destino já existe",
                                string.Format("Arquivo já existe: {0} [Arquivo destino já existe]", Path.Combine(caminhoDatR, arqDest))
                            );
                        }
                    } else {
                        this._email.EnviarEmail(
                            "Arquivo não convertido COBOL",
                            string.Format("Arquivo não convertido de: {0} para: {1}, [arquivo não convertido COBOL]", fileName, Path.Combine(caminhoArqTemp, "saida.txt"))
                        );
                    }
                }
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
