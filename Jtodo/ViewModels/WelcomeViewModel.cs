using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Jtodo.Commands;
using Jtodo.Domains;
using Jtodo.Interfaces;
using Jtodo.Services;

namespace Jtodo.ViewModels
{
    public class WelcomeViewModel : ViewModelBase
    {
        private readonly TodoListService _todoListService;
        private readonly INavigationService _navigationService;
        private ObservableCollection<object> _displayItems;

        public ObservableCollection<TodoList> TodoLists { get; set; }
        
        public ObservableCollection<object> DisplayItems
        {
            get => _displayItems;
            set
            {
                _displayItems = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToDetailCommand { get; }
        public ICommand CreateNewTodoListCommand { get; }

        public WelcomeViewModel(INavigationService navigationService, TodoListService todoListService)
        {
            _todoListService = todoListService;
            _navigationService = navigationService;
            TodoLists = new ObservableCollection<TodoList>();
            _displayItems = new ObservableCollection<object>();
            
            NavigateToDetailCommand = new RelayCommand(OnNavigateToDetail);
            CreateNewTodoListCommand = new RelayCommand(OnCreateNewTodoList);
        }

        public override async Task InitializeAsync()
        {
            await LoadTodoListsAsync();
        }

        private async Task LoadTodoListsAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading List Todo...";
                var lists = await Task.Run(() => _todoListService.Get_All_Todo_list());
                TodoLists.Clear();
                foreach (var list in lists)
                {
                    TodoLists.Add(list);
                }

                RefreshDisplayItems();
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error: {ex.Message}",
                    "Invalid",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void RefreshDisplayItems()
        {
            DisplayItems.Clear();
            
            DisplayItems.Add(new NewTodoListPlaceholder());

            foreach (var list in TodoLists)
            {
                DisplayItems.Add(list);
            }
        }

        private void OnNavigateToDetail(object? parameter)
        {
            if (parameter is TodoList todoList)
            {
                _navigationService.NavigateToDetail(todoList.Id.ToString());
            }
        }

        private void OnCreateNewTodoList(object? parameter)
        {
            System.Windows.MessageBox.Show("Create New Todo List - Feature coming soon!", "Info", 
            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
