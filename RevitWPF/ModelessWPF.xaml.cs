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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitWPF
{
    /// <summary>
    /// Interaction logic for ModelessWPF.xaml
    /// </summary>
    public partial class ModelessWPF : Window
    {
        private EventHandlerWithStringArg _mExternalMethodStringArg;
        private EventHandlerWithWpfArg _mExternalMethodWpfArg;

        public ModelessWPF(EventHandlerWithStringArg evExternalMethodStringArg, EventHandlerWithWpfArg eExternalMethodWpfArg)
        {
            InitializeComponent();
            _mExternalMethodStringArg = evExternalMethodStringArg;
            _mExternalMethodWpfArg = eExternalMethodWpfArg;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //Before the form is closed, everything must be disposed properly
            _mExternalMethodStringArg._revitEvent.Dispose();
            _mExternalMethodStringArg._revitEvent = null;
            _mExternalMethodStringArg = null;

            _mExternalMethodWpfArg._revitEvent.Dispose();
            _mExternalMethodWpfArg._revitEvent = null;
            _mExternalMethodWpfArg = null;

            //You have to call the base class
            base.OnClosing(e);
        }

        private void MakeRequest(RevitRequestId requestId)
        {
            _mExternalMethodWpfArg.RevitRequest.Make(requestId);
            _mExternalMethodWpfArg.Raise(this);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MakeRequest(RevitRequestId.CountWalls);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MakeRequest(RevitRequestId.CreateRandomWall);

        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            MakeRequest(RevitRequestId.BatchWalls);

        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            MakeRequest(RevitRequestId.ExportImage);

        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }


    }
}
