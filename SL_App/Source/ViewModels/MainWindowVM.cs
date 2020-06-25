using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
        public string TimerText { get; set; }

        public ObservableCollection<string> TableCollection { get; set; }

        public string SelectedTable { get; set; }
    }
}
