using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorBackup.Services
{
    public class CopyObject
    {
        public static readonly string PathApp = "./TempBackup/";

        Logger log = new Logger();

        public string CallCopyObject(string sourcePath)
        {
            try
            {
                if (System.IO.File.Exists(sourcePath))
                {
                    Console.WriteLine("This is File");
                    return copyFile(sourcePath);
                }
                else if (System.IO.Directory.Exists(sourcePath))
                {
                    Console.WriteLine("This is directory");

                    return copyDirectory(sourcePath);
                }
                else
                {
                    Console.WriteLine("Erorr Path");
                    return null;
                }
            }
            catch(Exception ex)
            {
                log.LogWrite($"Error in  metod CallCopyObject : {ex.Message}", Logger.typeLog.Error);
                return null;
            }
            
        }

        private string copyFile(string sourcePathFile, string destinationDirectoty = "")
        {
            try
            {
                if (_stringNullOrEmpty(destinationDirectoty))
                {
                    destinationDirectoty = PathApp;
                }

                Console.WriteLine(destinationDirectoty);

                FileInfo fileInf = new FileInfo(sourcePathFile);

                if (fileInf.Exists)
                {

                    //получить имя файла из исходного пути и меняем его розширение(Extension) 
                    string destinationFileName = Path.GetFileNameWithoutExtension(sourcePathFile) + "_" + DateTime.Now.ToString("yyyyMMddTHHmmss") + Path.GetExtension(sourcePathFile);
                    Console.WriteLine(destinationFileName);
                    //Формируем полный путь  
                    string destinationFilePath = Path.Combine(destinationDirectoty, destinationFileName);
                    Console.WriteLine(destinationFilePath);
                    fileInf.CopyTo(destinationFilePath);
                    // иницыализируем метод для проверки
                    DirectoryInfo destinationDirectotyInfo = new DirectoryInfo(destinationDirectoty);
                    var zip = Path.ChangeExtension(destinationFilePath, "zip");
                    //проверяем сущеcтвует ли директория
                    if (destinationDirectotyInfo.Exists)
                    {
                        // Создаем зип архив
                        using (ZipArchive archive = ZipFile.Open(zip, ZipArchiveMode.Create))
                        {
                            archive.CreateEntryFromFile(destinationFilePath, Path.GetFileName(destinationFilePath));
                        }
                        Console.WriteLine(destinationFilePath);
                        string fullPath = Path.GetFullPath(zip);

                        if (!_stringNullOrEmpty(fullPath))
                        {
                            Console.WriteLine(fullPath);
                            File.Delete(destinationFilePath);
                        }

                        return fullPath;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.LogWrite($"Error in  metod copyFile : {ex.Message}", Logger.typeLog.Error);
                log.LogTelegramm($"Error in  metod copyFile : {ex.Message}", Logger.typeLog.Error);
                return null;
            }
          

        }

        private string copyDirectory(string sourcePathDirectory, string destinationDirectoty = "")
        {
            //Проверка строки на нулевое значение и не пустое ли значение 
            if (_stringNullOrEmpty(destinationDirectoty))
            {
                destinationDirectoty = PathApp;
            }

            DirectoryInfo destinationDirectotyInfo = new DirectoryInfo(destinationDirectoty);
            //Проверка Сущесвует ли директория если нет то возращаем null
            if (destinationDirectotyInfo.Exists)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(sourcePathDirectory);
                //Проверка Сущесвует ли директория если нет то возращаем null
                if (directoryInfo.Exists)
                {
                    //Получить имя последней папки  в source_directory пути
                    string _directoryName = new DirectoryInfo(sourcePathDirectory).Name + "_" + DateTime.Now.ToString("yyyyMMddTHHmmss");

                    //Собираем Путь куда сохранить теневую копию ФАЙЛа
                    destinationDirectoty = Path.Combine(destinationDirectoty, _directoryName);
                    ////Создаем путь
                    System.IO.Directory.CreateDirectory(destinationDirectoty);

                    Console.WriteLine(destinationDirectoty);

                    var diSource = new DirectoryInfo(sourcePathDirectory);
                    var diDestination = new DirectoryInfo(destinationDirectoty);

                    string backupPath = directoryCopyAll(diSource, diDestination);

                    DirectoryInfo directoryBackupPath = new DirectoryInfo(backupPath);
                    try
                    {
                        if (directoryBackupPath.Exists)
                        {
                            var zip = Path.Combine(Path.ChangeExtension(backupPath, "zip"));
                            Console.WriteLine(zip);
                            ZipFile.CreateFromDirectory(backupPath, zip);

                            string fullPath = Path.GetFullPath(zip);
                            if (!_stringNullOrEmpty(fullPath))
                            {
                                deleteFolder(backupPath);
                            }

                            return fullPath;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch(Exception ex )
                    {
                        log.LogWrite($"Error in  metod copyDirectory : {ex.Message}", Logger.typeLog.Error);
                        log.LogTelegramm($"Error in  metod copyDirectory : {ex.Message}", Logger.typeLog.Error);
                        return null;
                    }
                    
                }
                else
                {
                    log.LogWrite($" Каталога: {sourcePathDirectory}  не существует ", Logger.typeLog.Error);
                    return null;
                }
            }
            else
            {
                log.LogWrite($" Каталога: {destinationDirectoty}  не существует ", Logger.typeLog.Error);
                log.LogTelegramm($" Каталога: {destinationDirectoty}  не существует ", Logger.typeLog.Error);
                return null;
            }


        }

        private string directoryCopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                Directory.CreateDirectory(target.FullName);
                // Copy each file into the new directory.
                foreach (FileInfo fi in source.GetFiles())
                {
                    Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }
                // Copy each subdirectory using recursion.
                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                    directoryCopyAll(diSourceSubDir, nextTargetSubDir);
                }
                return target.FullName;
            }
            catch (Exception ex)
            {
                log.LogWrite($"Error in  metod directoryCopyAll : {ex.Message}", Logger.typeLog.Error);
                log.LogTelegramm($"Error in  metod directoryCopyAll : {ex.Message}", Logger.typeLog.Error);
                return null;
            }
         
        }

        private string CompressZip(string sourceFile, string compressedFile)
        {
            try
            {
                // поток для чтения исходного файла
                using (System.IO.FileStream sourceStream = new System.IO.FileStream(sourceFile, System.IO.FileMode.OpenOrCreate))
                {
                    // поток для записи сжатого файла
                    using (System.IO.FileStream targetStream = File.Create(compressedFile))
                    {
                        // поток архивации
                        using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                        {
                            sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                            Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                                sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());

                        }
                    }
                }
                return compressedFile;
            }
            catch (Exception ex)
            {
                log.LogWrite($"Error in  metod CompressZip : {ex.Message}", Logger.typeLog.Error);
                return null;
            }
         
        }

        private void deleteFolder(string folder)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                DirectoryInfo[] diA = di.GetDirectories();
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo f in fi)
                {

                    //https://stackoverflow.com/questions/265896/how-do-i-delete-a-read-only-file
                    //Cогласно документации File.Delete, вам придется удалить атрибут только для чтения.Вы можете установить атрибуты файла с помощью File.SetAttributes().
                    //Эквивалент, если вы работаете с FileInfoобъектом:
                    f.IsReadOnly = false;

                    f.Delete();
                }
                foreach (DirectoryInfo df in diA)
                {
                    deleteFolder(df.FullName);
                }
                if (di.GetDirectories().Length == 0 && di.GetFiles().Length == 0) di.Delete();
            }
            catch (Exception ex)
            {
                log.LogWrite($"Error in  metod deleteFolder : {ex.Message}", Logger.typeLog.Error);
                log.LogTelegramm($"Error in  metod deleteFolder : {ex.Message}", Logger.typeLog.Error);
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }

        private bool isDirectoryContainFiles(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return false;
                return Directory.EnumerateFiles(path, "*", System.IO.SearchOption.AllDirectories).Any();

            }
            catch (Exception ex)
            {
                log.LogWrite($"Error in  metod isDirectoryContainFiles : {ex.Message}", Logger.typeLog.Error);
                log.LogTelegramm($"Error in  metod isDirectoryContainFiles : {ex.Message}", Logger.typeLog.Error);
                return false;
            }
           
        }

        private bool _stringNullOrEmpty(string s)
        {
            try
            {
                bool result;
                result = s == null || s == string.Empty;
                return result;
            }
            catch (Exception ex)
            {
                log.LogWrite($"Error in  metod stringNullOrEmpty : {ex.Message}", Logger.typeLog.Error);
                log.LogTelegramm($"Error in  metod stringNullOrEmpty : {ex.Message}", Logger.typeLog.Error);
                return false;
            }
         
        }
    }
}

