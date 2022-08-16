using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorBackup.Services
{
    //Реализация SaveChange из кода
    public class NotifierService
    {
        public Action OnSet { get; set; }

        private int _counter { get; set; } = 0;
        public int Counter
        {
            get => _counter;
            set
            {
                _counter = value;
                OnSet?.Invoke();
            }
        }


    }
}
