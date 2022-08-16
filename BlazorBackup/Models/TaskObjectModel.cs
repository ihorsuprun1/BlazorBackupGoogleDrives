using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorBackup.Models
{
    public class TaskObjectModel : JobBackupModel
    {
        [JsonIgnore]
        public Action OnPlannedTasksChange { get; set; }
        [Required(ErrorMessage = "Имя нужно обязательно!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Имя должно быть от 3 до 100 символов")]
        public string Name { get; set; } = "test";//string.Empty;

        [Required(ErrorMessage = "Нужно заполнить")]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [Range(0.0001, double.MaxValue, ErrorMessage = "Событие не может быть в прошлом...")]
        public double Interval { get; set; } = 1;

        [JsonIgnore]
        public int ConterLaunch { get; set; } = 0;

        [JsonIgnore]
        public bool Execution { get; set; } = false;
        //Не записывать в базу если этабаза json
        [JsonIgnore]
        public TaskObject taskObject { get; set; }
        //[JsonIgnore]
        //private bool test = false;
        //public bool Test
        //{
        //    get
        //    {
        //        return test;
        //    }
        //    set
        //    {
        //        //Реализация SaveChange из кода
        //        OnPlannedTasksChange?.Invoke();
        //        test = value;
        //    }
        //}
        //Не записывать в базу если этабаза sql >> using System.ComponentModel.DataAnnotations.Schema;
        //[NotMapped]
    }
}


