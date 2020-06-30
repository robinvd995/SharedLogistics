using SL_App.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SL_App.Windows
{
    /// <summary>
    /// Interaction logic for EmailPreviewWindow.xaml
    /// </summary>
    public partial class EmailPreviewWindow : Window
    {
        private EmailWindowVM _context;

        public EmailPreviewWindow(List<EmailWrapper> emails)
        {
            InitializeComponent();
            _context = EmailWindowVM.Create(emails);
            OnEmailChanged();
            DataContext = _context;
        }

        private void ButtonClickPrevious(object sender, RoutedEventArgs e)
        {
            if (_context.PrevEmail())
                OnEmailChanged();
        }

        private void ButtonClickNext(object sender, RoutedEventArgs e)
        {
            if (_context.NextEmail())
                OnEmailChanged();
        }

        private void OnEmailChanged()
        {
            webBrowser.NavigateToString(_context.CurrentEmail.HTMLBody);
        }

        private void ButtonClickConfirm(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
