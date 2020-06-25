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
                TableCollection = new ObservableCollection<string>(tables),
                TableContent = null

            };

            _timer = new SimpleTimer(1000, true, Execute);
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

        private void ShowTableClick(object sender, RoutedEventArgs e)
        {
            MainWindowVM context = DataContext as MainWindowVM;
            string selectedTable = context.SelectedTable;
            ISqlResultSet resultSet = _sqlManager.ExecuteParameterizedQuerry("SELECT * FROM dbo.{0}", new string[] { selectedTable });
            context.TableContent = CreateGridFromResultSet(resultSet);
        }

        private Grid CreateGridFromResultSet(ISqlResultSet dataset)
        {
            int columns = dataset.GetColumnCount();
            int rows = dataset.GetRowCount();

            Grid grid = new Grid();

            for (int i = 0; i < columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            }

            for (int i = 0; i < rows + 1; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }

            Thickness partialThickness = new Thickness(0, 0, 1, 1);
            Thickness topThickness = new Thickness(0, 1, 1, 1);
            Thickness leftThickness = new Thickness(1, 0, 1, 1);
            Thickness fullThickness = new Thickness(1, 1, 1, 1);

            for(int i = 0; i < columns; i++)
            {
                string colName = dataset.GetColumnName(i);
                Border border = new Border() { BorderBrush = Brushes.Black, BorderThickness = i == 0 ? fullThickness : topThickness, Background = Brushes.Gray };
                border.SetValue(Grid.ColumnProperty, i);
                border.SetValue(Grid.RowProperty, 0);
                TextBlock textBlock = new TextBlock() { Text = colName, Padding = new Thickness(5) };
                border.Child = textBlock;
                grid.Children.Add(border);
            }

            for (int i = 0; i < columns; i++)
            {
                for(int j = 0; j < rows; j++)
                {
                    Border border = new Border() { BorderBrush = Brushes.Black, BorderThickness = i == 0 ? leftThickness : partialThickness };
                    border.SetValue(Grid.ColumnProperty, i);
                    border.SetValue(Grid.RowProperty, j + 1);

                    string text = dataset.GetValue(i, j).AsString();
                    TextBlock textBlock = new TextBlock() { Text = text, Padding = new Thickness(5) };
                    border.Child = textBlock;
                    grid.Children.Add(border);
                }
            }
            
            return grid;
        }
    }
}
