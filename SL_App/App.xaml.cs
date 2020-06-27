using Newtonsoft.Json;
using SL_App.Source;
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

        public AppSettings Settings;

        public App()
        {
            Settings = LoadSettings();

            Console.WriteLine(Settings.TimerInterval + "," + Settings.TimerActive);

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
            return JsonConvert.DeserializeObject<AppSettings>(json);
        }

        public SqlManager SqlManager { get; }

        
    }

    public class AppSettings
    {
        public bool TimerActive { get; set; }
        public bool TimerLooping { get; set; }
        public int TimerInterval { get; set; }
        public int TimerMultiplier { get; set; }
    }
}
