using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.ViewModels
{
    public class EmailWindowVM : ViewModelBase
    {
        private List<EmailVM> _emails;
        private int _curEmail = 0;

        public EmailVM CurrentEmail { get; set; }
        public bool CanNext { get; set; }
        public bool CanPrev { get; set; }

        public bool NextEmail()
        {
            if(_curEmail + 1 < _emails.Count)
            {
                _curEmail++;
                CanNext = _curEmail + 1 < _emails.Count;
                CanPrev = true;
                OnEmailChanged();
                return true;
            }

            return false;
        }

        public bool PrevEmail()
        {
            if(_curEmail > 0)
            {
                _curEmail--;
                CanNext = true;
                CanPrev = _curEmail > 0;
                OnEmailChanged();
                return true;
            }

            return false;
        }

        private void OnEmailChanged()
        {
            CurrentEmail = _emails[_curEmail];
            OnPropertyChanged("CurrentEmail");
            OnPropertyChanged("CanNext");
            OnPropertyChanged("CanPrev");
        }

        public static EmailWindowVM Create(List<EmailWrapper> emails)
        {
            EmailWindowVM windowVM = new EmailWindowVM()
            {
                _emails = new List<EmailVM>(),
                CanPrev = false,
                CanNext = emails.Count > 1
            };

            foreach (EmailWrapper email in emails)
            {
                windowVM._emails.Add(new EmailVM(email));
            }

            windowVM.CurrentEmail = windowVM._emails[0];

            return windowVM;
        }
    }

    public class EmailVM : ViewModelBase
    {
        private EmailWrapper _source;

        public EmailVM(EmailWrapper src)
        {
            _source = src;
        }

        public bool ShouldSend
        {
            get
            {
                return _source.ShouldSend;
            }
            set
            {
                _source.ShouldSend = value;
                OnPropertyChanged("ShouldSend");
            }
        }

        public string Receiver
        {
            get
            {
                return _source.Receiver;
            }
            set
            {
                _source.Receiver = value;
                OnPropertyChanged("Receiver");
            }
        }

        public string Subject
        {
            get
            {
                return _source.Subject;
            }
            set
            {
                _source.Subject = value;
                OnPropertyChanged("Subject");
            }
        }

        public string HTMLBody
        {
            get
            {
                return _source.HTMLBody;
            }
            set
            {
                _source.HTMLBody = value;
                OnPropertyChanged("HTMLBody");
            }
        }
    }
}
