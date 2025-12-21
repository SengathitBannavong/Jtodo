using System.Windows.Controls;
using Jtodo.ViewModels;


namespace Jtodo.Views
{
    public partial class HomeView : Page
    {
        private readonly HomeViewModel _vm;

        public HomeView()
        {
            InitializeComponent();
            _vm = new HomeViewModel();
            DataContext = _vm;
        }
    }
}
