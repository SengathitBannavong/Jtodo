using System.Windows;


namespace Jtodo.Views
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Navigate(new HomeView());
        }
    }
}