using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SL_App.Util
{
    public class OutlookApplication
    {
        private Microsoft.Office.Interop.Outlook.Application _application;

        private OutlookApplication()
        {
            
        }

        public bool IsOpen { get; private set; }

        public bool CreateEmail(out OutlookEmail email)
        {
            App app = Application.Current as App;

            int totalAttempts = Math.Max(app.Settings.EmailCreateAttempts, 1);

            int attempts = 0;
            bool success = false;
            OutlookEmail outlookEmail = new OutlookEmail();
            while (attempts < totalAttempts && !success)
            {
                try
                {
                    var mail = _application.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem) as Microsoft.Office.Interop.Outlook.MailItem;
                    ((Microsoft.Office.Interop.Outlook.ItemEvents_10_Event)mail).Send += outlookEmail.OutlookEmailSend;
                    outlookEmail._mailItem = mail;
                    success = true;
                }
                catch (Exception e)
                {
                    Thread.Sleep(app.Settings.EmailThreadSleepTime);
                    Console.WriteLine("Error creating outlook application: {0}", e.Message);
                }

                attempts++;
            }

            email = outlookEmail;
            return success;
        }

        public static OutlookApplication CreateApplication()
        {
            App app = Application.Current as App;

            int totalAttempts = Math.Max(app.Settings.EmailCreateAttempts, 1);

            OutlookApplication application = new OutlookApplication();

            bool success = false;
            int attempts = 0;
            while (attempts < totalAttempts && !success)
            {
                try
                {
                    Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                    application._application = outlookApp;
                    success = true;
                }
                catch (Exception e)
                {
                    Thread.Sleep(app.Settings.OutlookThreadSleepTime);
                    Console.WriteLine("Error creating outlook application: {0}", e.Message);
                }

                attempts++;
            }

            application.IsOpen = success;
            return application;
        }
    }

    public class OutlookEmail
    {
        internal Microsoft.Office.Interop.Outlook.MailItem _mailItem;

        internal OutlookEmail()
        {
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

        internal void OutlookEmailSend(ref bool Cancel)
        {
            IsSent = true;
        }

        public void SendEmail()
        {
            _mailItem.Send();
        }

        public bool IsSent { get; set; }
    }
}
