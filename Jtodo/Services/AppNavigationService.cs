using System.Windows.Navigation;
using Jtodo.Services.Interfaces;
using Jtodo.Views;

namespace Jtodo.Services
{
    public class AppNavigationService : INavigationService
    {
        private NavigationService? _navigationService;

        public void SetNavigationService(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void NavigateToDetail(string listId)
        {
            _navigationService?.Navigate(new DetailPage(listId));
        }

        public void GoBack()
        {
            _navigationService?.GoBack();
        }
    }
}