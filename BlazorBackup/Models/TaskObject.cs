using BlazorBackup.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;


namespace BlazorBackup.Models
{
    public class TaskObject : IDisposable
    {
        //https://stackoverflow.com/questions/475763/is-it-necessary-to-dispose-system-timers-timer-if-you-use-one-in-your-applicatio/2880574
               private TaskObjectModel _taskData { get; set; }
        Logger logger = new Logger();
        private Timer _timer { get; set; }
        private string m_exePath = string.Empty;

        public TaskObject(TaskObjectModel tom)
        {
            try
            {
                _taskData = tom;
                DateTime baseDateTime = DateTime.Now;

                while (_taskData.DateCreated < baseDateTime)
                {
                    _taskData.DateCreated = _taskData.DateCreated.AddMilliseconds(_taskData.Interval);
                }

                Console.WriteLine("Correction DONE!!!");
                // Используется из постанства имен >>> using using System.Timers;
                _timer = new Timer((_taskData.DateCreated - baseDateTime).TotalMilliseconds);
                _timer.Elapsed += async (sender, e) => await TriggerAlarm();

            }
            catch(Exception ex)
            {
                logger.LogTelegramm($"Error ctor classa TaskObject :  {ex.Message} ", Logger.typeLog.Error);
                logger.LogWrite($"Error ctor classa TaskObject :  {ex.Message} ", Logger.typeLog.Error);
            }
         

        }

        public void StartTimer()
        {
            _timer.Start();
        }

        private async Task TriggerAlarm()
        {
            try
            {
                _timer.Interval = _taskData.Interval;
                Console.WriteLine("Alarm!");
                var t = Task.Run(() => RunTask());
                await t;
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Error class TaskObject TriggerAlarm()  :  {ex.Message} ", Logger.typeLog.Error);
                logger.LogTelegramm($"Error class TaskObject TriggerAlarm()  :  {ex.Message} ", Logger.typeLog.Error);
            }
        }

       

        public void RunTask()
        {
            try 
            {
                _taskData.ConterLaunch++;
                _taskData.Execution = true;
                
                CopyObject copyObject = new CopyObject();
                var _path = copyObject.CallCopyObject(_taskData.pathBackup);

                GoogleDriveApiServiceAccount googleDrive = new GoogleDriveApiServiceAccount();
                string returnId = googleDrive.UploadFileToDrive(_path, _taskData.nameFolderId);
                if (returnId != null || returnId != "")
                {
                    FileInfo fileInf = new FileInfo(_path);
                    if (fileInf.Exists)
                    {
                        Console.WriteLine(_path + "   deleted");
                        fileInf.Delete();
                       
                        Telegramm telegram = new Telegramm();
                        telegram.telegramAsync("Upload file >>> " + returnId);
                    }
                    else
                    {
                        logger.LogWrite("Error in RunTask", Logger.typeLog.Error);
                        logger.LogTelegramm("Error in RunTask", Logger.typeLog.Error);
                    }
                }



                var df = googleDrive.CleanDriveFiles(_taskData.nameFolderId, _taskData.keepBackupTime);
                if (df != null && df.Count > 0)
                {
                    foreach (var f in df)
                    {
                        Console.WriteLine(f.Name + "deleted");
                        logger.LogWrite("Delete file >>> " + f.Name, Logger.typeLog.Info);
                        logger.LogTelegramm("Delete file >>> " + f.Name, Logger.typeLog.Info);
                    }
                }
                else
                {
                    Console.WriteLine("Файлов для удаления нету");
                }


                Console.WriteLine($"{_taskData.ConterLaunch} :Task {_taskData.Name} is running every {_timer.Interval} miliseconds");
                Console.WriteLine($"NameJOb: {_taskData.Name} :nameFolder {_taskData.nameFolder} pathBackup: {_taskData.pathBackup}");
                _taskData.Execution = false;
                
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Error class TaskObject RunTask()  :  {ex.Message} ", Logger.typeLog.Error);
                logger.LogTelegramm($"Error class TaskObject RunTask()  :  {ex.Message} ", Logger.typeLog.Error);
            }
    
        }

        #region Disposed Timer 
        /// <summary>
        /// /https://metanit.com/sharp/tutorial/8.2.php
        /// </summary>
        private bool disposed = false;

        // реализация интерфейса IDisposable.
        public void Dispose()
        {
            try
            {
                Console.WriteLine("!!!!!!!!!!!!Dispose!!!!!!!!!!!!!!!!!");
                Dispose(true);
                // подавляем финализацию
                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Error class TaskObject Dispose()  :  {ex.Message} ", Logger.typeLog.Error);
                logger.LogTelegramm($"Error class TaskObject Dispose()  :  {ex.Message} ", Logger.typeLog.Error);
            }
           
        }
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        _timer.Dispose();
                        Console.WriteLine($"Task {_taskData.Name} DISPOSED HER TIMER!");
                    }
                    // освобождаем неуправляемые объекты
                    disposed = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Error class TaskObject Dispose(bool disposing)  :  {ex.Message} ", Logger.typeLog.Error);
                logger.LogTelegramm($"Error class TaskObject Dispose(bool disposing)  :  {ex.Message} ", Logger.typeLog.Error);
            }
           
        }
        ~TaskObject()
        {
            try
            {
                Dispose(false);
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Error class TaskObject ~TaskObject()  :  {ex.Message} ", Logger.typeLog.Error);
                logger.LogTelegramm($"Error class TaskObject ~TaskObject()  :  {ex.Message} ", Logger.typeLog.Error);
            }
          
        }
        #endregion
    }
}
