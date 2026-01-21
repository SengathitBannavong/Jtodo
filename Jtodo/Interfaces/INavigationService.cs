using System.Windows.Navigation;

namespace Jtodo.Interfaces
{
    public interface INavigationService
    {
        void NavigateToDetail(string listId);
        void GoBack();
        void SetNavigationService(NavigationService navigationService);
    }
}