using System.Windows.Controls;
using Jtodo.ViewModels;

namespace Jtodo.Views
{
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
            var viewModel = new WelcomeViewModel(App.NavigationService, App.TodoListService);
            DataContext = viewModel;
            Loaded += async (s, e) =>
            {
                await viewModel.InitializeAsync();
            };
        }
    }
}
