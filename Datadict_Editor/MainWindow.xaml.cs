using System.Windows;
using System.Windows.Input;


namespace Datadict_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();
            // Set the Data Context of the window to a new instance of a MainWindowViewModel
            this.DataContext = new MainWindowViewModel();
        }

 
    }
}
