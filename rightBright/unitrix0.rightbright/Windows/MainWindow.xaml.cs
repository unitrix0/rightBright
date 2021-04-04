using System.Windows;
using FontAwesome.WPF;

namespace unitrix0.rightbright.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FontAwesomeIcon FontAwesomeIcon => FontAwesomeIcon.Desktop;
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
