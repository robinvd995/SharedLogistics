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
using System.Threading;
using SL_App.Util;
using SL_App.Windows;

namespace SL_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly object ThreadLock = new object();

        private readonly SimpleTimer _timer;

        public MainWindow()
        {
            Thread.CurrentThread.Name = "UIThread";
            InitializeComponent();

            App app = Application.Current as App;
            IEnumerable<string> tables = app.SqlManager.Connect("SharedLogistics");

            DataContext = new MainWindowVM()
            {
                TimerText = string.Format("Timer: {0}{1}", app.Settings.TimerInterval, app.Settings.TimerMultiplier),
                TableCollection = new ObservableCollection<string>(tables),
                TableContent = null,
                EmailAdress = app.Settings.EmailAdress

            };

            if (app.Settings.TimerActive)
            {
                _timer = new SimpleTimer(app.Settings.ActualTimerInterval, app.Settings.TimerLooping, ExecuteTimer);
                _timer.StartTimer();
            }
        }

        public void ExecuteTimer()
        {
            lock (ThreadLock)
            {
                Dispatcher.Invoke(delegate () {
                    SendEmails();
                });
            }
        }

        private void SendEmails()
        {
            App app = Application.Current as App;

            if (app.SqlManager.IsConnected)
            {
                // Getting the entries where prealert is 0 / false
                ISqlResultSet result = app.SqlManager.ExecuteQuerryFromFile("Sql/GetPreAlertFalse.sql");
                if (result.GetRowCount() == 0)
                {
                    Console.WriteLine("No entries with PreAlert = false found!");
                    return;
                }

                string json = File.ReadAllText(App.TRANSFORM_DIR + "PurchaseOrder.json");
                TableTransformer tableTransformer = TableTransformer.FromJson(json);
                ISqlResultSet transformedResult = tableTransformer.TransformSqlResultSet(result);

                json = File.ReadAllText(App.TRANSFORM_DIR + "Items.json");
                tableTransformer = TableTransformer.FromJson(json);

                HashSet<int> ids = new HashSet<int>();
                for (int i = 0; i < result.GetRowCount(); i++)
                {
                    int? nid = result.GetValue(0, i).AsInt();
                    if (nid.HasValue)
                    {
                        ids.Add(nid.Value);
                    }
                }

                List<EmailWrapper> emails = new List<EmailWrapper>();

                for (int i = 0; i < result.GetRowCount(); i++)
                {
                    int idcol = result.ColumnIndexOf("ID");
                    int colid = result.GetValue(idcol, i).AsInt().Value;
                    int inboundcol = result.ColumnIndexOf("INBOUNDID");
                    int inboundid = result.GetValue(inboundcol, i).AsInt().Value;
                    ISqlResultSet itemResult = app.SqlManager.ExectuteParameterizedQuerryFromFile("Sql/ItemLevel.sql", new string[] { inboundid.ToString() });
                    ISqlResultSet transformedItemResult = tableTransformer.TransformSqlResultSet(itemResult);

                    EmailDataContext context = new EmailDataContext
                    {
                        ShipmentTable = SimpleHTMLTable.FromSqlResult(transformedResult, i, 1),
                        ItemTable = SimpleHTMLTable.FromSqlResult(transformedItemResult)
                    };

                    string htmlSource = File.ReadAllText("Html/EmailTemplate.html");
                    HTMLParser parser = new HTMLParser(htmlSource)
                    {
                        DataContext = context
                    };
                    string parsedSource = parser.Parse();
                    
                    EmailWrapper emailData = new EmailWrapper
                    {
                        ID = colid,
                        ShouldSend = true,
                        Receiver = app.Settings.EmailAdress,
                        Subject = ("PREALERT - OrderID=" + inboundid),
                        HTMLBody = parsedSource
                    };

                    emails.Add(emailData);
                }

                if (app.Settings.ShowEmailsBeforeSending)
                {
                    EmailPreviewWindow emailPreviewWindow = new EmailPreviewWindow(emails);
                    emailPreviewWindow.ShowDialog();
                }

                HashSet<int> sendIds = new HashSet<int>();

                OutlookApplication application = OutlookApplication.CreateApplication();
                foreach (EmailWrapper data in emails)
                {
                    if (data.ShouldSend)
                    {
                        //Console.WriteLine("EmailData: ID:{0}, TO:{1}, SUBJECT:{2}, SHOULD_SEND:{3}", data.ID, data.Receiver, data.Subject, data.ShouldSend);
                        bool sent = SendEmail(application, data);
                        if (sent)
                        {
                            sendIds.Add(data.ID);
                        }
                    }
                }

                if (sendIds.Count > 0)
                {
                    // updating the prealert to 1 / true
                    StringBuilder conditionBuilder = new StringBuilder();

                    int j = 0;
                    foreach (int id in sendIds)
                    {
                        conditionBuilder.Append("ID = " + id);
                        j++;
                        if (j < sendIds.Count)
                        {
                            conditionBuilder.Append(" OR ");
                        }
                    }

                    string updateQuerry = "UPDATE TRITPurchaseOrder SET PREALERT = 1 WHERE " + conditionBuilder.ToString();
                    int rowsAffected = app.SqlManager.ExecuteWithoutResult(updateQuerry);
                    Console.WriteLine("Rows affected: {0}", rowsAffected);
                }
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

            for (int i = 0; i < columns; i++)
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
                for (int j = 0; j < rows; j++)
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

        public bool SendEmail(OutlookApplication application, EmailWrapper emailData)
        {
            bool success = application.CreateEmail(out OutlookEmail email);
            if (success)
            {
                email.SetTo(emailData.Receiver);
                email.SetSubject(emailData.Subject);
                email.SetHTMLBody(emailData.HTMLBody);
                email.SendEmail();
                return true;
            }
            else
            {
                return false;
            }

        }

        private void WindowClosed(object sender, EventArgs e)
        {
            if (_timer.IsRunning)
                _timer.StopTimer();
        }

        private void ConfirmEmailClick(object sender, RoutedEventArgs e)
        {
            MainWindowVM context = DataContext as MainWindowVM;
            App app = Application.Current as App;

            string prevEmail = app.Settings.EmailAdress;
            if (prevEmail != null && prevEmail.Length > 0 && prevEmail.Equals(context.EmailAdress))
            {
                MessageBoxButton button = MessageBoxButton.OK;
                string caption = "Email Changed";
                string text = string.Format("Email Adress has not changed!");
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(text, caption, button, icon);
            }
            else
            {
                app.Settings.EmailAdress = context.EmailAdress;
                app.SaveSettings();

                MessageBoxButton button = MessageBoxButton.OK;
                string caption = "Email Changed";
                string text = string.Format("Email Adress has been changed from {0}, to {1}!", prevEmail, context.EmailAdress);
                MessageBoxImage icon = MessageBoxImage.Information;
                MessageBox.Show(text, caption, button, icon);
            }
        }
    }

    public class EmailDataContext
    {
        public IHTMLTableSchema ShipmentTable { get; set; }
        public IHTMLTableSchema ItemTable { get; set; }
    }

    public class EmailWrapper
    {
        public int ID { get; set; }
        public bool ShouldSend { get; set; }
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string HTMLBody { get; set; }
    }
}
