using System;
using System.Collections.Generic;

namespace BlazorBackup.Models
{
    public class TaskObjectLists
    {
        public List<TaskObjectModel> ListTskObjs { get; set; } /*= new List<TaskObjectModel>();*/
        //public Action OnPlannedTasksChange { get; set; }

        //private List<TaskObjectModel> listTskObjs;
        //public List<TaskObjectModel> ListTskObjs
        //{
        //    get
        //    {
        //        return listTskObjs;    // возвращаем значение свойства
        //    }
        //    set
        //    {
        //        listTskObjs = value;
        //        //OnPlannedTasksChange?.Invoke();
        //        // устанавливаем новое значение свойства
        //    }

        //}

    }
}
