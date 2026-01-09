using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Jtodo.Commands;
using Jtodo.Domains;
using Jtodo.Services;
using Jtodo.Services.Interfaces;

namespace Jtodo.ViewModels
{
    public class WelcomeViewModel : INotifyPropertyChanged
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
                RaisePropertyChanged();
            }
        }

        public ICommand NavigateToDetailCommand { get; }
        public ICommand CreateNewTodoListCommand { get; }

        public WelcomeViewModel(INavigationService navigationService)
        {
            _todoListService = new TodoListService();
            _navigationService = navigationService;
            TodoLists = new ObservableCollection<TodoList>();
            _displayItems = new ObservableCollection<object>();
            
            NavigateToDetailCommand = new RelayCommand(OnNavigateToDetail);
            CreateNewTodoListCommand = new RelayCommand(OnCreateNewTodoList);

            LoadTodoLists();
        }

        // Constructor สำหรับ backward compatibility
        public WelcomeViewModel() : this(new AppNavigationService())
        {
        }

        private void LoadTodoLists()
        {
            var lists = _todoListService.Get_All_Todo_list();
            TodoLists.Clear();
            foreach (var list in lists)
            {
                TodoLists.Add(list);
            }
            RefreshDisplayItems();
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
