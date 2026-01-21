using Jtodo.DTOs;
using Jtodo.Interfaces;
using Jtodo.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Jtodo.ViewModels
{
    /// <summary>
    /// ViewModel for Detail Page
    /// ✅ ใช้ DTOs และ load TodoItems จาก DTO
    /// </summary>
    public class DetailPageViewModel : ViewModelBase
    {
        private readonly TodoListService _todoListService;
        private readonly INavigationService _navigationService;
        private TodoListDto? _currentTodoList;
        private ObservableCollection<TodoItemDto> _todoItems;
        private string _listTitle;
        private string _listDescription;
        private ulong _listId;

        public ObservableCollection<TodoItemDto> TodoItems
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

        public TodoListDto? CurrentTodoList
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
            _todoItems = new ObservableCollection<TodoItemDto>();
            _listTitle = string.Empty;
            _listDescription = string.Empty;
        }

        public async Task LoadTodoListByIdAsync(ulong todoListId)
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading Detail....";

                var todoListDto = await _todoListService.Get_Todo_List_Async(todoListId);

                if (todoListDto != null)
                {
                    CurrentTodoList = todoListDto;
                    ListId = todoListDto.Id;
                    ListTitle = todoListDto.Title;
                    ListDescription = todoListDto.Description;

                    TodoItems.Clear();
                    if (todoListDto.TodoItems != null && todoListDto.TodoItems.Count > 0)
                    {
                        foreach (var itemDto in todoListDto.TodoItems)
                        {
                            TodoItems.Add(itemDto);
                        }
                        
                        Console.WriteLine($"[INFO] Loaded {TodoItems.Count} TodoItems for TodoList ID {todoListId}");
                    }
                    else
                    {
                        Console.WriteLine($"[WARNING] TodoList ID {todoListId} has no TodoItems");
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
                    
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [Obsolete("Use LoadTodoListByIdAsync instead")]
        public void LoadTodoListById(ulong todoListId)
        {
            _ = LoadTodoListByIdAsync(todoListId);
        }

        public void NavigateBack()
        {
            _navigationService.GoBack();
        }
    }
}
