using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.ComponentModel;
using yGet.EDFParser;

namespace EDFViewer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        
        private EdfViewModel edfViewModel = null;
        public Window1()
        {
            InitializeComponent();
            edfViewModel = new EdfViewModel();
            this.DataContext = edfViewModel;
        }

         private void Button_Click(object sender, RoutedEventArgs e)
        {
          
            OpenFileDialog browseReport = new OpenFileDialog();
            browseReport.DefaultExt = ".xlsx";
            browseReport.Filter = "Reports(.edf)|*.Edf";

            Nullable<bool> result = browseReport.ShowDialog();
            if (result == true)
            {
                edfViewModel.EDFFileName = browseReport.FileName;
                edfViewModel.InitializeEDFParser(browseReport.FileName);  
            }
        }

         private void OnCheckDrawingSheetName(object sender, RoutedEventArgs e)
         {

         }

         private void OnCheckFunctionBlock(object sender, RoutedEventArgs e)
         {

         }

         private void OnCheckDrawingSheetComment(object sender, RoutedEventArgs e)
         {

         }
    }

    public class EdfViewModel: INotifyPropertyChanged
    {
        private String _edfFileName = String.Empty;
        private bool _IsDrawingSheetNamePresent;
        EDFParser edfParser;

        public EdfViewModel()
        {
            edfParser = null;
        }
        public String EDFFileName
        {
            get
            {
                return _edfFileName;
            }
            set
            {
                _edfFileName = value;
                if (null != _edfFileName)
                {
                    OnPropertyChanged("EDFFileName");
                }
            }
        }

        public void InitializeEDFParser(String fileName)
        {
            if (null != edfParser)
            {
                edfParser.Dispose();  
            }
            edfParser = new EDFParser(fileName);   

        }

        public bool IsDrawingSheetNamePresent
        {
            get { return _IsDrawingSheetNamePresent; }
            set
            {
                _IsDrawingSheetNamePresent = value;
                OnPropertyChanged("IsDrawingSheetNamePresent");
            }
        }
        
        
        
        
        public event PropertyChangedEventHandler PropertyChanged;



        private void OnPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
