using Newtonsoft.Json;
using SL_App.SQL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SL_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string SETTINGS_DIR = "Settings/";
        public static readonly string TRANSFORM_DIR = "Transformers/";

        public AppSettings Settings { get; private set; }

        //public OutlookApplication Outlook { get; private set; }

        private Dictionary<string, int> _timerMultipliers = new Dictionary<string,int>()
        {
            { "ms", 1 },
            { "milisec", 1 },
            { "milisecond", 1 },
            { "miliseconds", 1 },
            { "s", 1000 },
            { "sec", 1000 },
            { "second", 1000 },
            { "seconds", 1000 },
            { "m", 60000 },
            { "min", 60000 },
            { "minute", 60000 },
            { "minutes", 60000 }
        };

        public App()
        {
            Settings = LoadSettings();
            //Outlook = new OutlookApplication();

            SqlManager = new SqlManager();
        }

        /*public void Email()
        {
            OutlookEmail email = new OutlookEmail();
            email.SetTo("robinvd95@live.nl");
            email.SetSubject("test");
            email.SetHTMLBody("<p>Test email!</p>");
            bool sent = email.Display(true);
            Console.WriteLine(sent);
        }*/

        private AppSettings LoadSettings()
        {
            string json = File.ReadAllText(SETTINGS_DIR + "settings.json");
            AppSettings settings = JsonConvert.DeserializeObject<AppSettings>(json);
            int multiplier = 1000;
            if(!(settings.TimerMultiplier == null || settings.TimerMultiplier.Length == 0))
            {
                _timerMultipliers.TryGetValue(settings.TimerMultiplier, out multiplier);
            }
            settings.ActualTimerInterval = settings.TimerInterval * multiplier;

            return settings;
        }

        public void SaveSettings()
        {
            string json = JsonConvert.SerializeObject(Settings);
            File.WriteAllText(SETTINGS_DIR + "settings.json", json);
        }

        public SqlManager SqlManager { get; }

        
    }

    public class AppSettings
    {
        public bool TimerActive { get; set; }
        public bool TimerLooping { get; set; }
        public int TimerInterval { get; set; }
        public string TimerMultiplier { get; set; }
        public bool ShowEmailsBeforeSending { get; set; }
        public string EmailAdress { get; set; }
        public int OutlookCreateAttempts { get; set; }
        public int EmailCreateAttempts { get; set; }
        public int OutlookThreadSleepTime { get; set; }
        public int EmailThreadSleepTime { get; set; }

        [JsonIgnore]
        public int ActualTimerInterval { get; set; }
    }
}
