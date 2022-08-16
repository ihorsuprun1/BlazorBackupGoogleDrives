using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorBackup.Services
{
    public class ReadJsonServiseAccount
    {
        public Action OnPlannedTasksChange { get; set; }
        Logger logger = new Logger();
        public readonly string FullPathJsonServiceAcaunt = Environment.CurrentDirectory + @"\JsonFile\servicebackup-ServiceAccount.json";//"./JsonFile/servicebackup-ServiceAccount.json";
        public string EmailServiceAccount { get; set; } = "Unknow";

        public async Task<string> ReadFromServiceAccount()
        {
            try
            {
                // Запускаем операцию чтения базы данных в отдельном потоке
                return await Task.Run(async () =>
                {

                    if (File.Exists(FullPathJsonServiceAcaunt) && FullPathJsonServiceAcaunt != null)
                    {

                        var streamReader = new StreamReader(FullPathJsonServiceAcaunt);

                        dynamic array = JsonConvert.DeserializeObject(streamReader.ReadToEnd());

                        EmailServiceAccount = array.client_email;
                        Console.WriteLine("{0} {1}", "EmailAdress ", EmailServiceAccount);




                        streamReader.Close();

                        //Реализация SaveChange из кода
                        OnPlannedTasksChange?.Invoke();
                        return EmailServiceAccount;
                    }
                    else
                    {
                        return null;
                    }

                });
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: ReadFromServiceAccount() >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod:  ReadFromServiceAccount() >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
          
        }
    }
}
