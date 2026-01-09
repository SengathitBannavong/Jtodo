using System.Windows.Controls;
using Jtodo.ViewModels;
using Jtodo.Services;
using Jtodo.Services.Interfaces;

namespace Jtodo.Views
{
    public partial class WelcomePage : Page
    {
        private readonly WelcomeViewModel _viewModel;
        private readonly INavigationService _navigationService;

        public WelcomePage()
        {
            InitializeComponent();
            _navigationService = new AppNavigationService();
            _viewModel = new WelcomeViewModel(_navigationService);
            DataContext = _viewModel;

            Loaded += (s, e) =>
            {
                if (_navigationService is AppNavigationService navService && NavigationService != null)
                {
                    navService.SetNavigationService(NavigationService);
                }
            };
        }
    }
}
