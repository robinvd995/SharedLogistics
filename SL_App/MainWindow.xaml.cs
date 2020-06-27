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
using SL_App.HTML;
using System.IO;

namespace SL_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SimpleTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            App app = Application.Current as App;
            IEnumerable<string> tables = app.SqlManager.Connect("SharedLogistics");

            DataContext = new MainWindowVM()
            {
                TimerText = "Timer: 5m",
                TableCollection = new ObservableCollection<string>(tables),
                TableContent = null

            };

            if (app.Settings.TimerActive)
            {
                _timer = new SimpleTimer(app.Settings.TimerInterval * app.Settings.TimerMultiplier, app.Settings.TimerLooping, ExecuteTimer);
                _timer.StartTimer();
            }
        }

        public void ExecuteTimer()
        {
            App app = Application.Current as App;

            if (app.SqlManager.IsConnected)
            {
                // Getting the entries where prealert is 0 / false
                ISqlResultSet result = app.SqlManager.ExecuteQuerryFromFile("Sql/GetPreAlertFalse.sql");
                if (result.GetRowCount() == 0)
                {
                    Console.WriteLine("Rows affected: 0");
                    return;
                }

                HashSet<int> ids = new HashSet<int>();
                for (int i = 0; i < result.GetRowCount(); i++)
                {
                    int? nid = result.GetValue(0, i).AsInt();
                    if (nid.HasValue)
                    {
                        ids.Add(nid.Value);
                    }
                }

                // Setting up the email
                string[] columnNames = new string[]
                {
                    "Job Number", "Relation Code", "ETA TIL", "ATA TIL", "Status", "PO Number", "Supplier Name", "Supplier City", "No. of SKU",
                    "Dims Count", "Line Item", "Pieces", "Total Gross Weight", "Total Netto Weight", "Total Value", "Shipment Ref Number", "Delivery By",
                    "Origin", "Destination", "Dangerous Goods", "UN Code", "Remarks", "Remarks Internal", "Commodity", "Our Ref", "Exempt No."
                };

                int[] columnIds = new int[]
                {
                    result.ColumnIndexOf("INBOUNDID"), result.ColumnIndexOf("RELATIONCODE"), result.ColumnIndexOf("INBOUNDDATE"), result.ColumnIndexOf("COLLECTIONDATE"),
                    result.ColumnIndexOf("STATUS_DEFAULT"), result.ColumnIndexOf("PO_NUMBER"), result.ColumnIndexOf("SUPPLIER_NAME"), result.ColumnIndexOf("SUPPLIER_CITY"),
                    result.ColumnIndexOf("DIMENSIONS_NO")
                };

                for(int i = 0; i < result.GetRowCount(); i++)
                {
                    /*string body = BuildEmailBody(result);
                    Console.WriteLine(body);
                    SendEmail(body);*/

                    
                    int inboundcol = result.ColumnIndexOf("INBOUNDID");
                    int inboundid = result.GetValue(inboundcol, i).AsInt().Value;
                    ISqlResultSet itemResult = app.SqlManager.ExectuteParameterizedQuerryFromFile("Sql/ItemLevel.sql", new string[] { inboundid.ToString() });

                    HTMLValueMapper mapper = new HTMLValueMapper();
                    mapper.ValueMap["ShipmentTable"] = SimpleHTMLTable.FromSqlResult(result, i, 1);
                    mapper.ValueMap["ItemTable"] = SimpleHTMLTable.FromSqlResult(itemResult);

                    string htmlSource = File.ReadAllText("Html/EmailTemplate.html");
                    HTMLParser parser = new HTMLParser(htmlSource, mapper);
                    string parsedSource = parser.Parse();
                    SendEmail(parsedSource);
                }

                // updating the prealert to 1 / true
                StringBuilder conditionBuilder = new StringBuilder();

                int j = 0;
                foreach (int id in ids)
                {
                    conditionBuilder.Append("ID = " + id);
                    j++;
                    if (j < ids.Count)
                    {
                        conditionBuilder.Append(" OR ");
                    }
                }

                string updateQuerry = "UPDATE TRITPurchaseOrder SET PREALERT = 1 WHERE " + conditionBuilder.ToString();
                int rowsAffected = app.SqlManager.ExecuteWithoutResult(updateQuerry);
                Console.WriteLine("Rows affected: {0}", rowsAffected);
            }
        }

        private void ShowTableClick(object sender, RoutedEventArgs e)
        {
            MainWindowVM context = DataContext as MainWindowVM;
            string selectedTable = context.SelectedTable;
            App app = Application.Current as App;
            ISqlResultSet resultSet = app.SqlManager.ExecuteParameterizedQuerry("SELECT * FROM dbo.{0}", new string[] { selectedTable });
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

        public void SendEmail(string emailBody)
        {
            EmailWindow window = new EmailWindow(emailBody);
            window.ShowDialog();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            if (_timer.IsRunning)
                _timer.StopTimer();
        }
    }
}
