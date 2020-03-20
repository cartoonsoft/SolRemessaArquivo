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

            string caminhoArqOrigemRemessa = this._configuration.GetValue<string>("CaminhoArqOrigemRemessa");
            string caminhoArqRemessa = this._configuration.GetValue<string>("CaminhoArqRemessa");
            string caminhoDatR = this._configuration.GetValue<string>("CaminhoDatR");
            string caminhoArqTemp = this._configuration.GetValue<string>("CaminhoArqTemp");
            string[] fileEntries = Directory.GetFiles(caminhoArqOrigemRemessa, "PORTAGCA.DAT*");

            foreach (string fileName in fileEntries)
            {
                if (File.Exists(fileName))
                {
                    string arqDest = "PORTAGCA_" + File.GetCreationTime(fileName).ToString("yyyyMMdd") +".DAT";
                    string arqEntrada = Path.Combine(caminhoArqTemp, "entrada.txt");

                    try
                    {
                        File.Copy(fileName, arqEntrada);
                        //ExeRun("C:\comp\COBOL34\conv.bat c:\tmp\entrada.txt c:\tmp\saida.txt", exeActive, exeWait)
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                        //EnviarEmailErro("Arquivo não copiado para temporário: "+AFile," [Arquivo não copiado para temporário]",0)
                    }

                    if (File.Exists(Path.Combine(caminhoArqTemp, "saida.txt")))
                    {
                        if (!File.Exists(Path.Combine(caminhoArqRemessa, arqDest)))
                        {
                            try
                            {
                                File.Copy(Path.Combine(caminhoArqTemp, "saida.txt"), Path.Combine(caminhoArqRemessa, arqDest));
                                File.Copy(Path.Combine(caminhoArqRemessa, arqDest), Path.Combine(caminhoDatR, arqDest));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                //EnviarEmailErro("Arquivo não copiado para: " + "\\192.168.60.150\cartorios01\jgomes\01-PROCESSAMENTO\ABN\"+nomefinal,"[Erro cópia para rede]",0)                            
                            }
                        } else {
                            //EnviarEmailErro("Arquivo já existe: " + "\\192.168.60.150\cartorios01\jgomes\01-PROCESSAMENTO\ABN\"+nomefinal,"[Arquivo destino já existe]",0)                         
                        }
                    } else {
                        //EnviarEmailErro("Arquivo não convertido: c:\tmp\entrada.txt", " [arquivo não convertido COBOL]", 0)
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
