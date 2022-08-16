using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorBackup.Services
{
    public class Logger
    {
        public enum typeLog
        {
            Info,
            Warning,
            Error

        }

        private string m_exePath = string.Empty;

        
        public void LogWrite(string logMessage , typeLog typeLog)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, w,typeLog);
                }
            }
            catch (Exception ex)
            {
                LogTelegramm($"Error Write LogFile {ex}", typeLog.Error);
            }
        }

        private void Log(string logMessage, TextWriter txtWriter, typeLog typeLog  )
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} >> {1} >> {2}", DateTime.Now.ToLongTimeString(), typeLog ,logMessage );
               
            }
            catch (Exception ex)
            {
                LogTelegramm($"Error Write LogFile {ex}", typeLog.Error);
            }
        }

        public void LogTelegramm(string logMessage, typeLog typeLog)
        {
            try
            {
                Telegramm telegram = new Telegramm();
                telegram.telegramAsync($"Log BlazorBackup: {DateTime.Now} >> {typeLog} >> {logMessage}");

            }
            catch (Exception ex)
            {
            }
        }
    }
}
