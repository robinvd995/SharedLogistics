using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App.Source
{
    public class OutlookEmail
    {
        private Microsoft.Office.Interop.Outlook.MailItem _mailItem;

        public OutlookEmail()
        {
            Microsoft.Office.Interop.Outlook.Application outlook = new Microsoft.Office.Interop.Outlook.Application();
            _mailItem = outlook.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem) as Microsoft.Office.Interop.Outlook.MailItem;

            ((Microsoft.Office.Interop.Outlook.ItemEvents_10_Event)_mailItem).Send += OutlookEmailSend;
        }

        public void SetTo(string adress)
        {
            _mailItem.To = adress;
        }

        public void SetSubject(string subject)
        {
            _mailItem.Subject = subject;
        }

        public void SetHTMLBody(string html)
        {
            _mailItem.HTMLBody = html;
        }

        public bool Display(bool display)
        {
            _mailItem.Display(display);
            return IsSent;
        }

        private void OutlookEmailSend(ref bool Cancel)
        {
            IsSent = true;
        }

        public bool IsSent { get; set; }
    }


}
