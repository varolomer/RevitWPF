using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace RevitWPF
{
    public class RevitExternalApp : IExternalApplication
    {
        //External application instance
        public static RevitExternalApp thisApp = null;

        //Form instance
        private ModelessWPF m_ModelessWPFForm;

        //Seperate thread to run the UI
        private Thread _uiThread;

        public Result OnShutdown(UIControlledApplication application)
        {
            if(m_ModelessWPFForm != null && m_ModelessWPFForm.Visibility == Visibility.Visible)
            {
                m_ModelessWPFForm.Close();
            }

            return Result.Succeeded;

        }

        public Result OnStartup(UIControlledApplication application)
        {
            m_ModelessWPFForm = null; //The command itself bring the form later on
            thisApp = this; //Static access must be given to reach the instance everywhere   

            //Create Ribbon Tab
            application.CreateRibbonTab("RevitWPF");
            RibbonPanel panelHelper = application.CreateRibbonPanel("RevitWPF", "RevitWPF");

            string path = Assembly.GetExecutingAssembly().Location;

            PushButtonData buttonEntry = new PushButtonData("Entry", "Entry", path, "RevitWPF.EntryCommand");
            BitmapImage iconEntry = new BitmapImage(new Uri("pack://application:,,,/RevitWPF;component/Assets/iconModelessForm.png"));
            PushButton pushButton_Entry = panelHelper.AddItem(buttonEntry) as PushButton;
            pushButton_Entry.LargeImage = iconEntry;

            PushButtonData buttonSeperateThread = new PushButtonData("Seperate Thread", "Seperate Thread", path, "RevitWPF.EntryCommandSeperateThread");
            PushButton pushButton_SeperateThread = panelHelper.AddItem(buttonSeperateThread) as PushButton;
            pushButton_SeperateThread.LargeImage = iconEntry;

            return Result.Succeeded;

        }

        public void ShowForm(UIApplication uiapp)
        {
            //If there is no UI instance yet, initialize and instance and show it
            if(m_ModelessWPFForm == null || PresentationSource.FromVisual(m_ModelessWPFForm) == null)
            {
                //Create external event instances with arguements
                EventHandlerWithStringArg evStr = new EventHandlerWithStringArg();
                EventHandlerWithWpfArg evWpf = new EventHandlerWithWpfArg();

                //Pass the instances to UI -- It is extremely important that UI is in charge of disposing these objects while closing
                m_ModelessWPFForm = new ModelessWPF(evStr, evWpf);
                m_ModelessWPFForm.Show();
            }
        }

        public void ShowFormSeperateThread(UIApplication uiapp)
        {
            // If we do not have a thread started or has been terminated start a new one
            if (!(_uiThread is null) && _uiThread.IsAlive) return;

            //Create external event instances with arguements
            EventHandlerWithStringArg evStr = new EventHandlerWithStringArg();
            EventHandlerWithWpfArg evWpf = new EventHandlerWithWpfArg();

            _uiThread = new Thread(() =>
            {
                //Set the sync context
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                //Initialize the form. -- It must be ensured that the instances passed
                //to the UI are disposed properly while closing
                m_ModelessWPFForm = new ModelessWPF(evStr, evWpf);

                //Shut down the dispatcher while closing
                m_ModelessWPFForm.Closed += (s, e) => Dispatcher.CurrentDispatcher.InvokeShutdown();

                m_ModelessWPFForm.Show();
                Dispatcher.Run();
            });

            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.IsBackground = true;
            _uiThread.Start();
        }
    }
}
