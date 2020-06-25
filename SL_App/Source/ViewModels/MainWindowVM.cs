using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SL_App.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
        private Grid _tableContent = null;

        public string TimerText { get; set; }

        public ObservableCollection<string> TableCollection { get; set; }

        public string SelectedTable { get; set; }

        public Grid TableContent
        {
            get
            {
                return _tableContent;
            }
            set
            {
                _tableContent = value;
                OnPropertyChanged("TableContent");
            }
        }
    }
}
