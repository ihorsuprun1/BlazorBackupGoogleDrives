using BlazorBackup.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
// Nuget install Google.Apis.Drive.v3
namespace BlazorBackup.Services
{
    public class GoogleDriveApiServiceAccount
    {
        //private const string PathToServiceAccountKeyFile = "./JsonFile/servicebackup-ServiceAccount.json";//@"D:\test\servicebackup-ServiceAccount.json";
        public readonly string PathToServiceAccountKeyFile = Environment.CurrentDirectory + @"\JsonFile\servicebackup-ServiceAccount.json";
        Logger logger = new Logger();

        private GoogleCredential GetUserCredential()
        {
            try
            {
                // Load the Service account credentials and define the scope of its access.
                return GoogleCredential.FromFile(PathToServiceAccountKeyFile)
                                .CreateScoped(DriveService.ScopeConstants.Drive);
            }
            catch(Exception ex)
            {
                logger.LogWrite($"Metod: GetUserCredential >> {ex.Message}" ,Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: GetUserCredential >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
           
        }

        private DriveService GetDriveService(GoogleCredential credential)
        {
            try
            {
                DriveService service = new DriveService(
               new BaseClientService.Initializer()
               {
                   HttpClientInitializer = credential,

               });
                service.HttpClient.Timeout = TimeSpan.FromMinutes(100);
                return service;
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: GetDriveService >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: GetUserCredential >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
           
        }
        //Определения расширения файла
        private string GetMimeType(string fileName)
        {
            if(fileName != null)
            {
                try
                {
                    string mimeType = "application/unknown";
                    string ext = System.IO.Path.GetExtension(fileName).ToLower();
                    Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                    if (regKey != null && regKey.GetValue("Content Type") != null)
                        mimeType = regKey.GetValue("Content Type").ToString();
                    return mimeType;

                }
                catch (Exception ex)
                {
                    logger.LogWrite($"Metod: GetUserCredential >> {ex.Message}", Logger.typeLog.Error);
                    logger.LogTelegramm($"Metod: GetUserCredential >> {ex.Message}", Logger.typeLog.Error);
                    return null;
                }
            }
            else
            {
                return null;
            }
           
           
        }
        //Загрузка файлов на диск
        public string UploadFileToDrive(string uploadFile, string GoogleDriveFolderId = "", string description = "Uploaded with BackupGoogleDrive.NET!")
        {
            try
            {
                //Авторизация и определения сервиса
                DriveService service = GetDriveService(GetUserCredential());
                //Создание файла метаданных
                var fileMatadata = new Google.Apis.Drive.v3.Data.File();//Создание файла  для гугл 
                fileMatadata.Name = System.IO.Path.GetFileName(uploadFile);//Достаем имя файла
                fileMatadata.Description = description;
                fileMatadata.MimeType = GetMimeType(uploadFile);
                //FolderId -  это id папки на гугл диске (узнать его можно зайти в эту же папку и в конце адресной стоке и будет id   https://drive.google.com/drive/u/0/folders/0B8tEClNsnw3Gd0E4ZWt4TTdUQVE 
                if (GoogleDriveFolderId != "")
                {
                    fileMatadata.Parents = new List<string> { GoogleDriveFolderId };
                    Console.WriteLine("папка на гугл диске id: {0}", GoogleDriveFolderId);
                }
                else
                {
                    Console.WriteLine("Папку на гугл диске не указали сохранятся будет в корневой каталог гугл диска ");
                }
                // статус загрузки
                Google.Apis.Upload.IUploadProgress progress;
                // выбор способа загрузки
                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(uploadFile, FileMode.Open))
                {
                    request = service.Files.Create(fileMatadata, stream, GetMimeType(uploadFile));
                    request.ProgressChanged += Request_ProgressChanged;
                    request.ResponseReceived += Request_ResponseReceived;
                    progress = request.Upload();
                }
                Console.WriteLine(progress.Status);
                // Вывод ошибок если они есть
                if (progress.Exception != null)
                {
                    Console.WriteLine(progress.Exception);
                }
                var file = request.ResponseBody;
                Console.WriteLine("ID файла на гугл диску: {0} имя файла: {1} ", file.Id, file.Name);
                return file.Id;
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: UploadFileToDrive >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: UploadFileToDrive >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
        }

        private void Request_ProgressChanged(Google.Apis.Upload.IUploadProgress obj)
        {
            Console.WriteLine(obj.Status + " " + obj.BytesSent);
        }

        private void Request_ResponseReceived(Google.Apis.Drive.v3.Data.File obj)
        {
            if (obj != null)
            {
                Console.WriteLine("File was uploaded sucessfully--" + obj.Id);
            }
        }

        public string DeleteFile(string fileID)
        {
            try
            {
                DriveService service = GetDriveService(GetUserCredential());
                FilesResource.DeleteRequest request = service.Files.Delete(fileID);

                return request.Execute();
            }
            catch(Exception ex)
            {
                logger.LogWrite($"не достаточно прав для удаления (Удалять файлы можно только те что создавались самим сервисным акаунтом) >> {ex.Message} ", Logger.typeLog.Error);
                logger.LogTelegramm($"не достаточно прав для удаления (Удалять файлы можно только те что создавались самим сервисным акаунтом) >> {ex.Message} ", Logger.typeLog.Error);
                return null;
            }
        }

        //public string createDirectory( string _title, string _description, string _parent)
        //{
        //    //bool exists = Exists(service, folderName);
        //    //if (exists)
        //    //    return $"Sorry but the file {folderName} already exists!";

        //    DriveService service = GetDriveService(GetUserCredential());

        //    var file = new Google.Apis.Drive.v3.Data.File();
        //    file.Name = folderName;
        //    file.MimeType = "application/vnd.google-apps.folder";
        //    var request = service.Files.Create(file);
        //    request.Fields = "id";
        //    var result = request.Execute();
        //    return result.Id;
        //}

        //public async Task<string> CreateFolderTask(string folderName, string GoogleDriveFolderId) 
        //{ 
        //    await Task.Run(() => CreateFolder(folderName, GoogleDriveFolderId));

        //    return string;
        //}

        public TaskObjectModel CreateFolder(TaskObjectModel tom)
        {
            try
            {
                Console.WriteLine("*********************************");
                DriveService service = GetDriveService(GetUserCredential());
                bool exists = Exists(tom.nameFolder, tom.parentfolderId);
                if (exists)
                {
                    Console.WriteLine($"Sorry but the file {tom.parentfolderId} already exists!");
                    return null;
                }

                var file = new Google.Apis.Drive.v3.Data.File();
                file.Name = tom.nameFolder;
                file.MimeType = "application/vnd.google-apps.folder";
                file.Parents = new List<string> { tom.parentfolderId };

                var request = service.Files.Create(file);

                request.Fields = "id";
                //Console.WriteLine("#####################" + request.Execute().Id + "#####################");
                tom.nameFolderId = request.Execute().Id;
                return tom;
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: CreateFolder(TaskObjectModel tom) >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: CreateFolder(TaskObjectModel tom) >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
            
        }

        public string CreateFolder(string folderName, string GoogleDriveFolderId)
        {
            try
            {
                Console.WriteLine("*********************************");
                DriveService service = GetDriveService(GetUserCredential());
                bool exists = Exists(folderName, GoogleDriveFolderId);
                if (exists)
                {
                    Console.WriteLine("Sorry but the file {folderName} already exists!");
                    return $"Sorry but the file {folderName} already exists!";
                }

                var file = new Google.Apis.Drive.v3.Data.File();
                file.Name = folderName;
                file.MimeType = "application/vnd.google-apps.folder";
                file.Parents = new List<string> { GoogleDriveFolderId };

                var request = service.Files.Create(file);

                request.Fields = "id";
                //Console.WriteLine("#####################" + request.Execute().Id + "#####################");
                return request.Execute().Id;
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: CreateFolder(string folderName, string GoogleDriveFolderId) >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: CreateFolder(string folderName, string GoogleDriveFolderId) >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
          
        }
      
        private bool Exists(string name, string GoogleDriveFolderId)
        {
            try
            {
                DriveService service = GetDriveService(GetUserCredential());
                var listRequest = service.Files.List();
                listRequest.PageSize = 100;
                listRequest.Q = $"trashed = false and name contains '{name}' and '{GoogleDriveFolderId}' in parents";
                listRequest.Fields = "files(name)";
                var files = listRequest.Execute().Files;

                foreach (var file in files)
                {
                    if (name == file.Name)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: Exists(string name, string GoogleDriveFolderId) >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: Exists(string name, string GoogleDriveFolderId) >> {ex.Message}", Logger.typeLog.Error);
                return false;
            }
            
        }
        //Вернуть файлы какие есть на гугл диске
        public List<GoogleDriveFile> GetDriveFiles()
        {
            try
            {
                DriveService service = GetDriveService(GetUserCredential());
                // Определяем параметры запроса.
                Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
                // только для получения папок
                //FileListRequest.Q = "mimeType='application/vnd.google-apps.folder'";

                //  для получения вхех параметров Fields
                FileListRequest.Fields = "nextPageToken, files(*)";

                // Список файлов.
                IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;

                List<GoogleDriveFile> FileList = new List<GoogleDriveFile>();


                // Для получения только папок
                // files = files.Where(x => x.MimeType == "application/vnd.google-apps.folder").ToList();


                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        GoogleDriveFile File = new GoogleDriveFile
                        {
                            Id = file.Id,
                            Name = file.Name,
                            Size = file.Size,
                            Version = file.Version,
                            CreatedTime = file.CreatedTime,
                            CreatedTimeRaw = file.CreatedTimeRaw

                        };
                        FileList.Add(File);
                        Console.WriteLine(file.Name);
                        Console.WriteLine(file.Id);
                    }
                }
                return FileList;
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: GetDriveFiles() >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: GetDriveFiles() >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
           
        }
        //Вернуть файлы какие есть на гугл диске
        public List<GoogleDriveFile> GetDriveFiles(bool folder)
        {
            try
            {
                DriveService service = GetDriveService(GetUserCredential());
                // Определяем параметры запроса.
                Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
                if (folder)
                {
                    // только для получения папок
                    FileListRequest.Q = "mimeType='application/vnd.google-apps.folder'";
                    FileListRequest.Fields = "nextPageToken, files(*)";
                }
                else
                {
                    //  для получения вхех параметров Fields
                    // FileListRequest.Q = "mimeType='application/vnd.google-apps.folder'";
                    FileListRequest.Fields = "nextPageToken, files(*)";
                }
                // Список файлов.
                IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files.Where(c => c.Trashed == false).ToList();

                List<GoogleDriveFile> FileList = new List<GoogleDriveFile>();

                // Для получения только папок
                // files = files.Where(x => x.MimeType == "application/vnd.google-apps.folder").ToList();

                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        GoogleDriveFile File = new GoogleDriveFile
                        {
                            Id = file.Id,
                            Name = file.Name,
                            Size = file.Size,
                            Version = file.Version,
                            CreatedTime = file.CreatedTime,
                            CreatedTimeRaw = file.CreatedTimeRaw
                        };
                        FileList.Add(File);
                        Console.WriteLine(file.Name);
                        Console.WriteLine(file.Id);
                    }
                }
                return FileList;
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: GetDriveFiles(bool folder) >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: GetDriveFiles(bool folder) >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
            
        }
        //Вернуть файлы какие есть на гугл диске
        public List<GoogleDriveFile> GetDriveFiles(string parentId)
        {
            try
            {
                DriveService service = GetDriveService(GetUserCredential());
                // Определяем параметры запроса.
                Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
                FileListRequest.Q = $"'{parentId}' in parents";

                
                // только для получения папок
                //FileListRequest.Q = "mimeType='application/vnd.google-apps.folder'";

                //  для получения вхех параметров Fields
                FileListRequest.Fields = "nextPageToken, files(*)";

                // Список файлов.
                IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;

                List<GoogleDriveFile> FileList = new List<GoogleDriveFile>();


                // Для получения только папок
                // files = files.Where(x => x.MimeType == "application/vnd.google-apps.folder").ToList();


                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        GoogleDriveFile File = new GoogleDriveFile
                        {
                            Id = file.Id,
                            Name = file.Name,
                            Size = file.Size,
                            Version = file.Version,
                            CreatedTime = file.CreatedTime,
                            CreatedTimeRaw = file.CreatedTimeRaw

                        };
                        FileList.Add(File);
                        Console.WriteLine(file.Name);
                        Console.WriteLine(file.Id);
                    }
                }
                return FileList;
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: GetDriveFiles(string parentId) >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: GetDriveFiles(string parentId) >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
           
        }
        //Вернуть файлы какие есть на гугл диске
        public List<GoogleDriveFile> CleanDriveFiles(string parentId, int keepTimeClean)
        {
            try
            {
                DriveService service = GetDriveService(GetUserCredential());
                // Определяем параметры запроса.
                Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
                FileListRequest.Q = $"'{parentId}' in parents";


                // только для получения файлов
                //FileListRequest.Q = "mimeType='application/vnd.google-apps.file'";


                //  для получения вхех параметров Fields
                FileListRequest.Fields = "nextPageToken, files(*)";

                // Список файлов.
                IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;

                List<GoogleDriveFile> FileList = new List<GoogleDriveFile>();
                List<GoogleDriveFile> MarkDeletionFileList = new List<GoogleDriveFile>();

                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        GoogleDriveFile File = new GoogleDriveFile
                        {
                            Id = file.Id,
                            Name = file.Name,
                            Size = file.Size,
                            Version = file.Version,
                            CreatedTime = file.CreatedTime,
                            CreatedTimeRaw = file.CreatedTimeRaw

                        };
                        FileList.Add(File);

                    }
                }

                foreach (var f in FileList)
                {
                    Console.WriteLine(f.Name);
                    Console.WriteLine(f.Id);
                    Console.WriteLine(f.CreatedTime);
                    Console.WriteLine(f.CreatedTimeRaw);
                    Console.WriteLine("Вызов удаления на гугл диске метод CleanDriveFiles");
                    var time = f.CreatedTime.Value.AddMinutes(keepTimeClean);//23:00 <22
                    Console.WriteLine( f.CreatedTime + "+" + keepTimeClean  + time + "  " + " DateTime.Now" + DateTime.Now);
                    if (time < DateTime.Now)
                    {
                        Console.WriteLine("delette  id   " + f.Id + "name : " + f.Name);
                        MarkDeletionFileList.Add(f);
                        DeleteFile(f.Id);
                    }
                }
                return MarkDeletionFileList;
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: CleanDriveFiles >> {ex.Message}", Logger.typeLog.Error);
                logger.LogTelegramm($"Metod: CleanDriveFiles >> {ex.Message}", Logger.typeLog.Error);
                return null;
            }
           
        }
    }
}


