using BlazorBackup.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace BlazorBackup.Services
{
    public class JsonServiceDB
    {
        public Action OnPlannedTasksChange { get; set; }
        public readonly string FullPathDB = "./LocalDB/db.json";
        public TaskObjectLists _taskObjectLists = new TaskObjectLists();
        Logger logger = new Logger();

        public TaskObjectLists argTaskObjectLists
        {
            get
            {
                if (_taskObjectLists.ListTskObjs.Count > 0) return _taskObjectLists;
                else return null;

            }
        }
        public Task<TaskObjectLists> ReadFromDB(bool async)
        {

            try
            {
                // Запускаем операцию чтения базы данных в отдельном потоке
                return Task.Run(async () =>
                {

                    if (File.Exists(FullPathDB) && FullPathDB != null)
                    {

                        using (FileStream fs = new FileStream(FullPathDB, FileMode.OpenOrCreate))
                        {
                            _taskObjectLists = await JsonSerializer.DeserializeAsync<TaskObjectLists>(fs);
                        }

                        if (_taskObjectLists.ListTskObjs.Count > 0)
                        {
                            foreach (var tsk in _taskObjectLists.ListTskObjs)
                            {
                                Console.WriteLine(tsk.Name);

                                TaskObject taskObject = new TaskObject(tsk);
                                tsk.taskObject = taskObject;

                                Console.WriteLine(@"Задача {0} ЗАПУЩЕНА", tsk.Name);
                                tsk.taskObject.StartTimer();
                            }
                        }

                        Console.WriteLine("JsonSerializer");

                        //Реализация SaveChange из кода
                        OnPlannedTasksChange?.Invoke();
                        return _taskObjectLists;
                    }
                    else
                    {
                        return null;
                    }

                });
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: ReadFromDB(bool async) >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: ReadFromDB(bool async) >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
        }

        public async Task<TaskObjectLists> ReadFromDB()
        {
            return await Task.Run(async () =>
            {
                
                //Реализация SaveChange из кода
                OnPlannedTasksChange?.Invoke();
                return _taskObjectLists;
            });

            //try
            //{
            //    // Запускаем операцию чтения базы данных в отдельном потоке
            //    return await Task.Run(async () =>
            //    {

            //        if (File.Exists(FullPathDB) && FullPathDB != null)
            //        {

            //            using (FileStream fs = new FileStream(FullPathDB, FileMode.OpenOrCreate))
            //            {
            //                _taskObjectLists = await JsonSerializer.DeserializeAsync<TaskObjectLists>(fs);
            //            }

            //            if (_taskObjectLists.ListTskObjs.Count > 0)
            //            {
            //                foreach (var tsk in _taskObjectLists.ListTskObjs)
            //                {
            //                    //Console.WriteLine(tsk.Name);

            //                    //TaskObject taskObject = new TaskObject(tsk);
            //                    //tsk.taskObject = taskObject;

            //                    //Console.WriteLine(@"Задача {0} ЗАПУЩЕНА", tsk.Name);
            //                    //tsk.taskObject.StartTimer();
            //                }

            //            }

            //            Console.WriteLine("JsonSerializer");

            //            //Реализация SaveChange из кода
            //            OnPlannedTasksChange?.Invoke();
            //            return _taskObjectLists;
            //        }
            //        else
            //        {
            //            return null;
            //        }

            //    });
            //}
            //catch (Exception ex)
            //{
            //    logger.LogWrite($"Metod: ReadFromDB() >> {ex.Message}", Logger.typeLog.Error);
            //    logger.LogTelegramm($"Metod: ReadFromDB() >> {ex.Message}", Logger.typeLog.Error);
            //    return null;
            //}
          
        }

        public async Task WriteToDB()
        {
            try
            {
                await Task.Run(() =>
                {
                    var serializerOptions = new JsonSerializerOptions
                    {
                        // Формирует вид, привлекательный для чтения и печати.
                        WriteIndented = true,

                        // Настройка кодировки символов для кирилицы.
                        // По умолчанию сериализатор выполняет escape - последовательность символов,
                        // отличных от ASCII.То есть он заменяет их \uxxxx,
                        // где xxxx является кодом Юникода символа.
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All, UnicodeRanges.Cyrillic)
                    };


                    string s = JsonSerializer.Serialize<TaskObjectLists>(_taskObjectLists, serializerOptions);
                    var sw = new StreamWriter(FullPathDB);
                    sw.Write(s);
                    sw.Close();
                });
                //Реализация SaveChange из кода
                OnPlannedTasksChange?.Invoke();
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod:  WriteToDB() >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod:  WriteToDB() >> {ex.Message}", Logger.typeLog.Error);
                
            }
          
        }

        public async Task AddTaskObject(TaskObjectModel tsk)
        {
            try
            {
                await Task.Run(async () =>
                {

                    TaskObject taskObject = new TaskObject(tsk);
                    tsk.taskObject = taskObject;

                    _taskObjectLists.ListTskObjs.Add(tsk);

                    await WriteToDB();

                    tsk.taskObject.StartTimer();

                });
                //Реализация SaveChange из кода
                OnPlannedTasksChange?.Invoke();
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod:  AddTaskObject(TaskObjectModel tsk) >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod:  AddTaskObject(TaskObjectModel tsk) >> {ex.Message}", Logger.typeLog.Error);
            }
        }
        public async Task DeletedJob(TaskObjectModel tsk)
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (tsk != null)
                    {
                        tsk.taskObject.Dispose();
                        _taskObjectLists.ListTskObjs.Remove(tsk);

                        Console.WriteLine($" Remove  {tsk.Name}");
                        await WriteToDB();
                    }
                });
                OnPlannedTasksChange?.Invoke();
                //Реализация SaveChange из кода
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod:  DeletedJob(TaskObjectModel tsk) >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod:  DeletedJob(TaskObjectModel tsk) >> {ex.Message}", Logger.typeLog.Error);
            }
        
        }


    }
}
