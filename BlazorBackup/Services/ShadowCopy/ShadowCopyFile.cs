//using Alphaleonis.Win32.Filesystem;
//using System;
//using System.Diagnostics;
//using System.IO.Compression;
//using System.Linq;
//using System.Threading;
//using Stream = System.IO.Stream;
////Nuget >> AlphaFS and AlphaVSS
////нужно добавить ссылку на DLL на сборку "System.IO.Compression.FileSystem.dll" 
////В обозревателе решений щелкните правой кнопкой мыши проект node и нажмите "Добавить ссылку".
////В диалоговом окне "Добавить ссылку" выберите вкладку, указывающую тип компонента, который вы хотите ссылаться.
////Выберите компоненты, которые вы хотите ссылаться, и нажмите кнопку "ОК".
////Из статьи MSDN Как добавить или удалить ссылки с помощью диалогового окна Добавить ссылку.
//namespace BlazorBackup.Service.ShadowCopy
//{
//    public class ShadowCopyFile
//    {
//        public static readonly string PathApp = "./TempBackup/";
//        //Для работы Shadow Copy требуются права админитратора если в VisualStudio то запуск студии от имени администратора
//        public   string BackupFile(string source_file)
//        {

//            // Путь к файлу какому нужно сделать теневую копию Пример
//            // string source_file = @"C:\Users\admin\Desktop\Audi_A7.xlsx";
//            Log.Write("Процес инициализации бекапа файла по пути:  " + source_file);
//            FileInfo fileInf = new FileInfo(source_file);
//            if (fileInf.Exists)
//            {
//                Log.Write("Имя файла: " + fileInf.Name + "     Размер: " + fileInf.Length + "    Время создания:  " + fileInf.CreationTime);

//                //получить имя файла из исходного пути
//                string fileName = Path.GetFileNameWithoutExtension(source_file) + "_" + DateTime.Now.ToString("yyyyMMddTHHmmss") + Path.GetExtension(source_file);



//                //Путь куда сохранить теневую копию ФАЙЛа В ЭТОМ КАТАЛОГЕ НЕ ДОЛЖНО БЫТЬ ФАЙЛА С ТАКИМ ИМЕНЕМ
//                string creadPath = PathApp;  //Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
//                //creadPath = Path.Combine(creadPath, "TempBackup");
               
//                //Создаем путь
//                //System.IO.Directory.CreateDirectory(creadPath);

//                Log.Write("Путь сохранения теневой копии: " + creadPath);
//                Console.WriteLine(creadPath);

//                string backup_path = Path.Combine(creadPath, fileName);
//                Console.WriteLine(backup_path);
//                // Инициализируйте подсистему теневого копирования.
//                using (VssBackup vss = new VssBackup())
//                {
//                    vss.Setup(System.IO.Path.GetPathRoot(source_file));
//                    string snap_path = vss.GetSnapshotPath(source_file);
//                    Console.WriteLine(snap_path);
//                    FileInfo fileBackup = new FileInfo(backup_path);
//                    if (fileBackup.Exists)
//                    {
//                        fileBackup.Delete();
//                        // Здесь мы используем библиотеку AlphaFS для создания копии Файла.
//                        Alphaleonis.Win32.Filesystem.File.Copy(snap_path, backup_path);
//                        string fileZip = Path.Combine(Path.ChangeExtension(backup_path, "zip"));
//                        CompressZip(backup_path, fileZip);
//                        string fullPath = Path.GetFullPath(fileZip);
//                        Console.WriteLine(fullPath);
//                        return fullPath;
//                    }
//                    else
//                    {
//                        // Здесь мы используем библиотеку AlphaFS для создания копии Файла.
//                        Alphaleonis.Win32.Filesystem.File.Copy(snap_path, backup_path);
//                        string fileZip = Path.Combine(Path.ChangeExtension(backup_path, "zip"));
//                        Console.WriteLine("Ждем пока архивацию  файла по пути:  " + fileBackup);
//                        CompressZip(backup_path, fileZip);
//                        string fullPath = Path.GetFullPath(fileZip);
//                        Console.WriteLine(fullPath);
//                        Console.WriteLine("Удаляем исходный файл :  " + backup_path);
//                        fileBackup.Delete();
//                        return fullPath;
//                        //return fileZip;
//                    }

//                }




//            }
//            else
//            {
//                Log.Write("Файла не существует или  указан не верный путь ");
//                return null;

//            }

//        }

//        public  string BackupDirectory(string source_directory)
//        {
//            Log.Write("Процес инициализации бекапа каталога по пути:  " + source_directory);

//            DirectoryInfo directoryInfo = new DirectoryInfo(source_directory);
//            if (directoryInfo.Exists)
//            {
//                //Получить имя последней папки  в source_directory пути
//                string _directoryName = new DirectoryInfo(source_directory).Name + "_" + DateTime.Now.ToString("yyyyMMddTHHmmss");

//                //Путь куда сохранить теневую копию ФАЙЛа
//                string creadPath = PathApp;
//                //string creadPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
//                creadPath = Path.Combine(creadPath , _directoryName);

//                ////Создаем путь
//                System.IO.Directory.CreateDirectory(creadPath);
//                Console.WriteLine(creadPath);

//                //string backup_path = Path.Combine(creadPath, Path.GetFileName(_directoryName));

//                // Инициализируйте подсистему теневого копирования.
//                using (VssBackup vss = new VssBackup())
//                {
//                    vss.Setup(System.IO.Path.GetPathRoot(source_directory));
//                    string snap_path = vss.GetSnapshotPath(source_directory);

//                    DirectoryInfo directoryCreatePath = new DirectoryInfo(creadPath);
//                    if (directoryCreatePath.Exists)
//                    {
//                        deleteFolder(creadPath);
//                        //Здесь мы используем библиотеку AlphaFS для создания копии ПАПКИ(directory).позволяет создавать shadowCopy  каталогов
//                        var AlphaDirectory = Alphaleonis.Win32.Filesystem.Directory.Copy(snap_path, creadPath);
//                        Console.WriteLine(AlphaDirectory.TotalBytes);
//                        Console.WriteLine(AlphaDirectory.TimestampsCopied);
//                        Console.WriteLine(AlphaDirectory.IsEmulatedMove);
//                        Console.WriteLine(AlphaDirectory.IsCanceled);
//                        Console.WriteLine(AlphaDirectory.TotalFolders);
//                        // https://metanit.com/sharp/tutorial/5.7.php ZipFile позволяет создавать архив из каталогов

//                        Console.WriteLine(isDirectoryContainFiles(creadPath));
//                        var zip = Path.Combine( Path.ChangeExtension(creadPath, "zip"));
//                        Console.WriteLine(zip);
//                        ZipFile.CreateFromDirectory(creadPath, zip);
//                        deleteFolder(creadPath);
//                        string fullPath = Path.GetFullPath(zip);

//                        return fullPath;

//                    }
//                    else
//                    {
//                        //Здесь мы используем библиотеку AlphaFS для создания копии ПАПКИ(directory).позволяет создавать shadowCopy  каталогов
//                        var AlphaDirectory = Alphaleonis.Win32.Filesystem.Directory.Copy(snap_path, creadPath);
//                        Console.WriteLine(AlphaDirectory.TotalBytes);
//                        Console.WriteLine(AlphaDirectory.TimestampsCopied);
//                        Console.WriteLine(AlphaDirectory.IsEmulatedMove);
//                        Console.WriteLine(AlphaDirectory.IsCanceled);
//                        Console.WriteLine(AlphaDirectory.TotalFolders);
//                        // https://metanit.com/sharp/tutorial/5.7.php ZipFile позволяет создавать архив из каталогов

//                        Console.WriteLine(isDirectoryContainFiles(creadPath));
//                        var zip = Path.Combine(Path.ChangeExtension(creadPath, "zip"));
//                        Console.WriteLine(zip);
//                        ZipFile.CreateFromDirectory(creadPath, zip);
//                        deleteFolder(creadPath);
//                        string fullPath = Path.GetFullPath(zip);

//                        return fullPath;

//                    }


//                }

//            }
//            else
//            {

//                Log.Write("Каталога по данному пути не существует: " + source_directory);
//                return null;
//            }



//        }

//        public  void deleteFolder(string folder)
//        {
//            try
//            {
//                DirectoryInfo di = new DirectoryInfo(folder);
//                DirectoryInfo[] diA = di.GetDirectories();
//                FileInfo[] fi = di.GetFiles();
//                foreach (FileInfo f in fi)
//                {

//                    //https://stackoverflow.com/questions/265896/how-do-i-delete-a-read-only-file
//                    //Cогласно документации File.Delete, вам придется удалить атрибут только для чтения.Вы можете установить атрибуты файла с помощью File.SetAttributes().
//                    //Эквивалент, если вы работаете с FileInfoобъектом:
//                    f.IsReadOnly = false;

//                    f.Delete();
//                }
//                foreach (DirectoryInfo df in diA)
//                {
//                    deleteFolder(df.FullName);
//                }
//                if (di.GetDirectories().Length == 0 && di.GetFiles().Length == 0) di.Delete();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Произошла ошибка: " + ex.Message);
//            }
//        }





//        public  bool isDirectoryContainFiles(string path)
//        {
//            if (!Directory.Exists(path)) return false;
//            return Directory.EnumerateFiles(path, "*", System.IO.SearchOption.AllDirectories).Any();
//        }

//        public  string CompressZip(string sourceFile, string compressedFile)
//        {
//            // поток для чтения исходного файла
//            using (System.IO.FileStream sourceStream = new System.IO.FileStream(sourceFile, System.IO.FileMode.OpenOrCreate))
//            {
//                // поток для записи сжатого файла
//                using (System.IO.FileStream targetStream = File.Create(compressedFile))
//                {
//                    // поток архивации
//                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
//                    {
//                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
//                        Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
//                            sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());

//                    }
//                }
//            }
//            return compressedFile;
//        }

//        //public static string CompressZip(string sourceFile, string compressedFile)
//        //{
//        //    // поток для чтения исходного файла
//        //    using (System.IO.FileStream sourceStream = new System.IO.FileStream(sourceFile, System.IO.FileMode.OpenOrCreate))
//        //    {
//        //        // поток для записи сжатого файла
//        //        using (System.IO.FileStream targetStream = File.Create(compressedFile))
//        //        {
//        //            // поток архивации
//        //            using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
//        //            {
//        //                sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
//        //                Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
//        //                    sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());

//        //            }
//        //        }
//        //    }
//        //    return compressedFile;
//        //}

//        public static void Example2()
//        {
//            // Этот код создает теневую копию и открывает поток поверх файла
//            // на новом томе моментального снимка.

//            string filename = @"C:\Users\admin\Desktop\SDelete.zip";
//            string backup_root = @"C:\Users\admin\Desktop\Backups\q.zip";


//            // Инициализируем подсистему теневого копирования.
//            using (VssBackup vss = new VssBackup())
//            {
//                Console.WriteLine(Path.GetPathRoot(filename));
//                vss.Setup(Path.GetPathRoot(filename));

//                // Теперь мы можем получить доступ к теневой копии, либо получив поток:
//                using (Stream s = vss.GetStream(filename))
//                {

//                    Debug.Assert(s.CanRead == true);
//                    Debug.Assert(s.CanWrite == false);

//                    using (var fileStream = new System.IO.FileStream(backup_root, System.IO.FileMode.Create))
//                    {
//                        s.CopyTo(fileStream);

//                    }

//                }
//            }

//        }


//    }
//}
