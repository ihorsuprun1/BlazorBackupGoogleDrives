using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorBackup.Services
{
    public class FileSearch
    {
        Logger log = new Logger();

        public List<string> files = new List<string>();

        public async Task<List<string>> GetFileAll(string sDir, string param = "*.*")
        {

            await Task.Run(() => DirSearch(sDir, param));

            return files;
        }

        public void DirSearch(string sDir, string param = "*.*")
        {
            try
            {
                foreach (string f in Directory.GetFiles(sDir, param))
                {
                    files.Add(f);
                    Console.WriteLine(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirSearch(d, param);
                }

            }
            catch (System.Exception ex)
            {
                log.LogWrite($"Error in  metod DirSearch : {ex.Message}", Logger.typeLog.Error);
                Console.WriteLine(ex.Message);
            }

        }
        public List<string> SearchFile(string sDir, string param = "*.*")
        {
            List<string> result = new List<string>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir, param))
                {
                    result.Add(f);
                    Console.WriteLine(f);
                }

                return result;
            }
            catch (System.Exception ex)
            {
                log.LogWrite($"Error in  metod SearchFile : {ex.Message}", Logger.typeLog.Error);
                Console.WriteLine(ex.Message);
                return null;
            }

        }
    }
}
