using System.ComponentModel.DataAnnotations;


namespace BlazorBackup.Models
{
    public class JobBackupModel
    {
        [Required(ErrorMessage = "Имя нужно обязательно!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Имя должно быть от 3 до 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Путь к папке бекапа!")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Путь должно быть от 2 до 100 символов")]
        public string pathBackup { get; set; }

        //[Required(ErrorMessage = "Имя нужно обязательно!")]
        //[StringLength(100, MinimumLength = 3, ErrorMessage = "Имя должно быть от 3 до 100 символов")]
        public string parentfolderId { get; set; }

        //[Required(ErrorMessage = "Имя нужно обязательно!")]
        //[StringLength(100, MinimumLength = 3, ErrorMessage = "Имя должно быть от 3 до 100 символов")]
        public string nameFolder { get; set; }

        //[Required(ErrorMessage = "Имя нужно обязательно!")]
        //[StringLength(100, MinimumLength = 3, ErrorMessage = "Имя должно быть от 3 до 100 символов")]
        public string nameFolderId { get; set; }

        public int keepBackupTime { get; set; }
    }
}
