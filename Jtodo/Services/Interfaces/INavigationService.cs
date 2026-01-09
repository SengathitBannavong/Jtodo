using System.Windows.Navigation;

namespace Jtodo.Services.Interfaces
{
    public interface INavigationService
    {
        void NavigateToDetail(string listId);
        void GoBack();
        void SetNavigationService(NavigationService navigationService);
    }
}