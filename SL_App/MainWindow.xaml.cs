using SL_App.ViewModels;
using SL_App.SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SL_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SimpleTimer _timer;
        private SqlManager _sqlManager;

        public MainWindow()
        {
            InitializeComponent();

            _sqlManager = new SqlManager();
            IEnumerable<string> tables = _sqlManager.Connect("SharedLogistics");

            DataContext = new MainWindowVM()
            {
                TimerText = "Timer: 5m",
                TableCollection = new ObservableCollection<string>(tables)
            };

            _timer = new SimpleTimer(1000, true, Execute);

            _sqlManager.ExecuteQuerry("SELECT * FROM dbo.TRITPurchaseOrder");
        }

        public static void Execute()
        {
            Console.WriteLine("Hello World!");
        }

        private void StartTimerClick(object sender, RoutedEventArgs e)
        {
            if(!_timer.IsRunning)
                _timer.StartTimer();
        }

        private void StopTimerClick(object sender, RoutedEventArgs e)
        {
            if(_timer.IsRunning)
                _timer.StopTimer();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            if (_timer.IsRunning)
                _timer.StopTimer();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(((MainWindowVM)DataContext).SelectedTable);
        }
    }
}
