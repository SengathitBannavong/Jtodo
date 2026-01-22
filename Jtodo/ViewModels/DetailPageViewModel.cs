using Jtodo.Commands;
using Jtodo.DTOs;
using Jtodo.Interfaces;
using Jtodo.Services;
using Jtodo.Values;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

namespace Jtodo.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
        private readonly TodoListService _todoListService;
        private readonly TodoItemService _todoItemService;
        private readonly TypeService _typeService;
        private readonly INavigationService _navigationService;
        private TodoListDto? _currentTodoList;
        private ObservableCollection<TodoItemDto> _todoItems;
        private ObservableCollection<Domains.Type> _availableTypes;
        private ObservableCollection<Values.Priority> _priorities;
        private ObservableCollection<Values.Status> _statuses;
        private string _listTitle;
        private string _listDescription;
        private ulong _listId;

        // Edit properties
        private bool _isEditingTitle;
        private bool _isEditingDescription;
        private string _editingTitle;
        private string _editingDescription;

        public ObservableCollection<TodoItemDto> TodoItems
        {
            get => _todoItems;
            set { _todoItems = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Domains.Type> AvailableTypes
        {
            get => _availableTypes;
            set { _availableTypes = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Values.Priority> Priorities
        {
            get => _priorities;
            set { _priorities = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Values.Status> Statuses
        {
            get => _statuses;
            set { _statuses = value; OnPropertyChanged(); }
        }

        public string ListTitle
        {
            get => _listTitle;
            set { _listTitle = value; OnPropertyChanged(); }
        }

        public string ListDescription
        {
            get => _listDescription;
            set { _listDescription = value; OnPropertyChanged(); }
        }

        public ulong ListId
        {
            get => _listId;
            set { _listId = value; OnPropertyChanged(); }
        }

        public TodoListDto? CurrentTodoList
        {
            get => _currentTodoList;
            private set { _currentTodoList = value; OnPropertyChanged(); }
        }

        // Edit properties
        public bool IsEditingTitle
        {
            get => _isEditingTitle;
            set { _isEditingTitle = value; OnPropertyChanged(); }
        }

        public string EditingTitle
        {
            get => _editingTitle;
            set { _editingTitle = value; OnPropertyChanged(); }
        }

        public bool IsEditingDescription
        {
            get => _isEditingDescription;
            set { _isEditingDescription = value; OnPropertyChanged(); }
        }

        public string EditingDescription
        {
            get => _editingDescription;
            set { _editingDescription = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand StartEditTitleCommand { get; }
        public ICommand StartEditDescriptionCommand { get; }
        public ICommand SaveTitleCommand { get; }
        public ICommand SaveDescriptionCommand { get; }
        public ICommand CancelEditTitleCommand { get; }
        public ICommand CancelEditDescriptionCommand { get; }
        public ICommand StartEditItemCommand { get; }
        public ICommand SaveEditItemCommand { get; }
        public ICommand AddTaskCommand { get; }

        public DetailPageViewModel(INavigationService navigationService, TodoListService todoListService, 
            TodoItemService todoItemService, TypeService typeService)
        {
            _navigationService = navigationService;
            _todoListService = todoListService;
            _todoItemService = todoItemService;
            _typeService = typeService;
            _todoItems = new ObservableCollection<TodoItemDto>();
            _availableTypes = new ObservableCollection<Domains.Type>();
            _priorities = new ObservableCollection<Values.Priority>(Enum.GetValues<Values.Priority>());
            _statuses = new ObservableCollection<Values.Status>(Enum.GetValues<Values.Status>());
            _listTitle = string.Empty;
            _listDescription = string.Empty;
            _editingTitle = string.Empty;
            _editingDescription = string.Empty;

            StartEditTitleCommand = new RelayCommand(p => StartEditTitle());
            StartEditDescriptionCommand = new RelayCommand(p => StartEditDescription());
            SaveTitleCommand = new RelayCommand(async p => await SaveTitleAsync());
            SaveDescriptionCommand = new RelayCommand(async p => await SaveDescriptionAsync());
            CancelEditTitleCommand = new RelayCommand(p => CancelEditTitle());
            CancelEditDescriptionCommand = new RelayCommand(p => CancelEditDescription());
            StartEditItemCommand = new RelayCommand(p => StartEditItem(p as TodoItemDto));
            SaveEditItemCommand = new RelayCommand(async p => await SaveEditItemAsync(p as TodoItemDto));
            AddTaskCommand = new RelayCommand(async p => await AddTaskAsync());
        }

        public async Task LoadTodoListByIdAsync(ulong todoListId)
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading Detail....";

                // Load types
                var types = await _typeService.GetAllTypesAsync();
                AvailableTypes.Clear();
                foreach (var type in types)
                {
                    AvailableTypes.Add(type);
                }

                var todoListDto = await _todoListService.Get_Todo_List_Async(todoListId);
                if (todoListDto != null)
                {
                    CurrentTodoList = todoListDto;
                    ListId = todoListDto.Id;
                    ListTitle = todoListDto.Title;
                    ListDescription = todoListDto.Description;

                    TodoItems.Clear();
                    if (todoListDto.TodoItems != null)
                    {
                        foreach (var item in todoListDto.TodoItems)
                        {
                            // Map TypeName for display
                            var type = AvailableTypes.FirstOrDefault(t => t.Id == item.TypeId);
                            if (type != null)
                            {
                                item.TypeName = type.Text;
                            }
                            
                            // Initialize as not editing
                            item.IsEditing = false;
                            
                            TodoItems.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}", "Error");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Title edit methods
        private void StartEditTitle()
        {
            EditingTitle = ListTitle;
            IsEditingTitle = true;
        }

        private async Task SaveTitleAsync()
        {
            if (string.IsNullOrWhiteSpace(EditingTitle))
            {
                System.Windows.MessageBox.Show("Title Cant Empty", "Warning");
                return;
            }

            try
            {
                if (CurrentTodoList != null)
                {
                    CurrentTodoList.Title = EditingTitle;
                    await _todoListService.Update_Todo_List_Async(CurrentTodoList);
                    ListTitle = EditingTitle;
                    IsEditingTitle = false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void CancelEditTitle()
        {
            EditingTitle = ListTitle;
            IsEditingTitle = false;
        }

        // Description edit methods
        private void StartEditDescription()
        {
            EditingDescription = ListDescription;
            IsEditingDescription = true;
        }

        private async Task SaveDescriptionAsync()
        {
            if(string.IsNullOrEmpty(EditingDescription))
            {
                System.Windows.MessageBox.Show("Description Cant Empty", "Warning");
                return;
            }

            try
            {
                if (CurrentTodoList != null)
                {
                    CurrentTodoList.Description = EditingDescription ?? string.Empty;
                    await _todoListService.Update_Todo_List_Async(CurrentTodoList);
                    ListDescription = EditingDescription ?? string.Empty;
                    IsEditingDescription = false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void CancelEditDescription()
        {
            EditingDescription = ListDescription;
            IsEditingDescription = false;
        }

        // TodoItem edit methods
        private void StartEditItem(TodoItemDto? item)
        {
            if (item == null) return;
            item.IsEditing = true;
            
            // Subscribe to property changes to update TypeName when TypeId changes
            item.PropertyChanged += OnItemPropertyChanged;
        }

        private void OnItemPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TodoItemDto.TypeId) && sender is TodoItemDto item)
            {
                // Update TypeName immediately when TypeId changes
                var type = AvailableTypes.FirstOrDefault(t => t.Id == item.TypeId);
                if (type != null)
                {
                    item.TypeName = type.Text;
                }
            }
        }

        private async Task SaveEditItemAsync(TodoItemDto? item)
        {
            if (item == null) return;

            // Validate before saving
            if (string.IsNullOrWhiteSpace(item.Title))
            {
                System.Windows.MessageBox.Show("Title cannot be empty", "Validation Error");
                return;
            }

            if (item.DueDate < item.StartDate)
            {
                System.Windows.MessageBox.Show("Due date cannot be before start date", "Validation Error");
                return;
            }

            try
            {
                // Update TypeName for display
                var type = AvailableTypes.FirstOrDefault(t => t.Id == item.TypeId);
                if (type != null)
                {
                    item.TypeName = type.Text;
                }

                // Save to database
                await _todoItemService.UpdateTodoItemAsync(item);
                
                // Also update the TodoList to keep it in sync
                if (CurrentTodoList != null)
                {
                    await _todoListService.Update_Todo_List_Async(CurrentTodoList);
                }

                item.IsEditing = false;
                
                // Unsubscribe from property changes
                item.PropertyChanged -= OnItemPropertyChanged;
                
                Console.WriteLine($"[INFO] Successfully saved TodoItem ID: {item.Id}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving item: {ex.Message}", "Error");
                Console.WriteLine($"[ERROR] Failed to save TodoItem: {ex.Message}");
            }
        }

        // Add new task method
        private async Task AddTaskAsync()
        {
            try
            {
                if (CurrentTodoList == null)
                {
                    System.Windows.MessageBox.Show("No TodoList is currently loaded", "Error");
                    return;
                }

                IsLoading = true;
                LoadingMessage = "Adding new task...";

                // Create new task in database
                var newTaskDto = await _todoListService.CreateTaskInListAsync(CurrentTodoList.Id);

                // Map TypeName for display
                var type = AvailableTypes.FirstOrDefault(t => t.Id == newTaskDto.TypeId);
                if (type != null)
                {
                    newTaskDto.TypeName = type.Text;
                }

                // Subscribe to property changes
                newTaskDto.PropertyChanged += OnItemPropertyChanged;

                // Set editing mode and clear title for immediate editing
                newTaskDto.IsEditing = true;
                newTaskDto.Title = "";

                // Add to collection
                TodoItems.Add(newTaskDto);

                // Also add to CurrentTodoList to keep it in sync
                if (CurrentTodoList.TodoItems != null)
                {
                    CurrentTodoList.TodoItems.Add(newTaskDto);
                }

                Console.WriteLine($"[INFO] Successfully added new task to UI");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error adding task: {ex.Message}", "Error");
                Console.WriteLine($"[ERROR] Failed to add task: {ex.Message}");
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