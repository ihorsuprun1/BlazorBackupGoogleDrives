using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorBackup
{
    class JsonServiceSetting
    {
        private readonly string settingsPath = Environment.CurrentDirectory + @"\Settings.json";

        public Settings ReadJsonConf()
        {
            if (new FileInfo(settingsPath).Length == 0)
            {
                WriteJsonConf();
            }
            string conf = File.ReadAllText(settingsPath);
            var settings = JsonSerializer.Deserialize<Settings>(conf);
            return settings;

        }
        private void WriteJsonConf()
        {
            
            Settings conf = new Settings();
            var json = JsonSerializer.Serialize<Settings>(conf);
            File.WriteAllText(settingsPath, json);
        }
    }
}
