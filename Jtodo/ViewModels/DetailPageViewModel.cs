using Jtodo.Domains;
using Jtodo.Interfaces;
using Jtodo.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Jtodo.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
        private readonly TodoListService _todoListService;
        private readonly INavigationService _navigationService;
        private TodoList? _currentTodoList;
        private ObservableCollection<TodoItem> _todoItems;
        private string _listTitle;
        private string _listDescription;
        private ulong _listId;

        public ObservableCollection<TodoItem> TodoItems
        {
            get => _todoItems;
            set
            {
                _todoItems = value;
                OnPropertyChanged();
            }
        }

        public string ListTitle
        {
            get => _listTitle;
            set
            {
                _listTitle = value;
                OnPropertyChanged();
            }
        }

        public string ListDescription
        {
            get => _listDescription;
            set
            {
                _listDescription = value;
                OnPropertyChanged();
            }
        }

        public ulong ListId
        {
            get => _listId;
            set
            {
                _listId = value;
                OnPropertyChanged();
            }
        }

        public TodoList? CurrentTodoList
        {
            get => _currentTodoList;
            private set
            {
                _currentTodoList = value;
                OnPropertyChanged();
            }
        }

        public DetailPageViewModel(INavigationService navigationService, TodoListService todoListService)
        {
            _navigationService = navigationService;
            _todoListService = todoListService;
            _todoItems = new ObservableCollection<TodoItem>();
            _listTitle = string.Empty;
            _listDescription = string.Empty;
        }

        public async Task LoadTodoListByIdAsync(ulong todoListId)
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading Detail....";

                var todoList = await Task.Run(() => _todoListService.Get_Todo_List(todoListId));

                if (todoList != null)
                {
                    CurrentTodoList = todoList;
                    ListId = todoList.Id;
                    ListTitle = todoList.Title;
                    ListDescription = todoList.Description;
                    TodoItems.Clear();
                    foreach (var item in todoList.Todo_Items)
                    {
                        TodoItems.Add(item);
                    }
                }
                else
                {
                    ListTitle = "Not Found Todo";
                    ListDescription = string.Empty;
                    
                    System.Windows.MessageBox.Show(
                        $"Not Found Todo with ID: {todoListId}",
                        "Invalid",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error Fetch Data: {ex.Message}",
                    "Invalid",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void NavigateBack()
        {
            _navigationService.GoBack();
        }
    }
}
