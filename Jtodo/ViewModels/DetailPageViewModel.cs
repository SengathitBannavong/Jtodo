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
using System.Windows.Data;
using System.ComponentModel;

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
        private ICollectionView _todoItemsView;
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
        private bool _isEditMode;

        // Filter properties
        private Priority? _selectedPriorityFilter;
        private Status? _selectedStatusFilter;
        private ulong? _selectedTypeFilter;
        private string _sortColumn;
        private bool _isSortAscending;

        public ObservableCollection<TodoItemDto> TodoItems
        {
            get => _todoItems;
            set { _todoItems = value; OnPropertyChanged(); }
        }

        public ICollectionView TodoItemsView
        {
            get => _todoItemsView;
            set { _todoItemsView = value; OnPropertyChanged(); }
        }

        public Priority? SelectedPriorityFilter
        {
            get => _selectedPriorityFilter;
            set
            {
                _selectedPriorityFilter = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public Status? SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                _selectedStatusFilter = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public ulong? SelectedTypeFilter
        {
            get => _selectedTypeFilter;
            set
            {
                _selectedTypeFilter = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public string CurrentSortColumn
        {
            get => _sortColumn;
            set { _sortColumn = value; OnPropertyChanged(); }
        }

        public bool IsSortAscending
        {
            get => _isSortAscending;
            set { _isSortAscending = value; OnPropertyChanged(); }
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

        public bool IsEditMode
        {
            get => _isEditMode;
            set { _isEditMode = value; OnPropertyChanged(); }
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
        public ICommand ToggleEditModeCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand SortByColumnCommand { get; }

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
            _sortColumn = string.Empty;
            _isSortAscending = true;

            // Initialize CollectionView
            _todoItemsView = CollectionViewSource.GetDefaultView(_todoItems);
            _todoItemsView.Filter = FilterTodoItems;

            StartEditTitleCommand = new RelayCommand(p => StartEditTitle());
            StartEditDescriptionCommand = new RelayCommand(p => StartEditDescription());
            SaveTitleCommand = new RelayCommand(async p => await SaveTitleAsync());
            SaveDescriptionCommand = new RelayCommand(async p => await SaveDescriptionAsync());
            CancelEditTitleCommand = new RelayCommand(p => CancelEditTitle());
            CancelEditDescriptionCommand = new RelayCommand(p => CancelEditDescription());
            StartEditItemCommand = new RelayCommand(p => StartEditItem(p as TodoItemDto));
            SaveEditItemCommand = new RelayCommand(async p => await SaveEditItemAsync(p as TodoItemDto));
            AddTaskCommand = new RelayCommand(async p => await AddTaskAsync());
            ToggleEditModeCommand = new RelayCommand(p => ToggleEditMode());
            DeleteTaskCommand = new RelayCommand(async p => await DeleteTaskAsync(p as TodoItemDto));
            ClearFilterCommand = new RelayCommand(p => ClearFilter());
            SortByColumnCommand = new RelayCommand(p => SortByColumn(p as string));
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
                            // Map TypeName and TypeColor for display
                            var type = AvailableTypes.FirstOrDefault(t => t.Id == item.TypeId);
                            if (type != null)
                            {
                                item.TypeName = type.Text;
                                // Convert int color to hex string (default gray if 0 or invalid)
                                item.TypeColor = type.Color != 0 ? $"#{type.Color:X6}" : "#9E9E9E";
                            }
                            else
                            {
                                item.TypeName = "Unknown";
                                item.TypeColor = "#9E9E9E"; // Default gray for unknown types
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
                // Update TypeName and TypeColor immediately when TypeId changes
                var type = AvailableTypes.FirstOrDefault(t => t.Id == item.TypeId);
                if (type != null)
                {
                    item.TypeName = type.Text;
                    // Convert int color to hex string (default gray if 0 or invalid)
                    item.TypeColor = type.Color != 0 ? $"#{type.Color:X6}" : "#9E9E9E";
                }
                else
                {
                    item.TypeName = "Unknown";
                    item.TypeColor = "#9E9E9E"; // Default gray
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
                // Update TypeName and TypeColor for display
                var type = AvailableTypes.FirstOrDefault(t => t.Id == item.TypeId);
                if (type != null)
                {
                    item.TypeName = type.Text;
                    // Convert int color to hex string (default gray if 0 or invalid)
                    item.TypeColor = type.Color != 0 ? $"#{type.Color:X6}" : "#9E9E9E";
                }
                else
                {
                    item.TypeName = "Unknown";
                    item.TypeColor = "#9E9E9E"; // Default gray
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

                // Map TypeName and TypeColor for display
                var type = AvailableTypes.FirstOrDefault(t => t.Id == newTaskDto.TypeId);
                if (type != null)
                {
                    newTaskDto.TypeName = type.Text;
                    // Convert int color to hex string (default gray if 0 or invalid)
                    newTaskDto.TypeColor = type.Color != 0 ? $"#{type.Color:X6}" : "#9E9E9E";
                }
                else
                {
                    newTaskDto.TypeName = "Unknown";
                    newTaskDto.TypeColor = "#9E9E9E"; // Default gray
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

        // Toggle Edit Mode
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        // Delete Task
        private async Task DeleteTaskAsync(TodoItemDto? item)
        {
            if (item == null) return;

            try
            {
                // Confirm deletion
                var result = System.Windows.MessageBox.Show(
                    $"Are you sure you want to delete '{item.Title}'?",
                    "Confirm Delete",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (result != System.Windows.MessageBoxResult.Yes)
                    return;

                IsLoading = true;
                LoadingMessage = "Deleting task...";

                // Delete from database
                await _todoItemService.DeleteTodoItemAsync(item.Id);

                // Remove from UI collection
                TodoItems.Remove(item);

                // Also remove from CurrentTodoList to keep it in sync
                if (CurrentTodoList?.TodoItems != null)
                {
                    var itemToRemove = CurrentTodoList.TodoItems.FirstOrDefault(i => i.Id == item.Id);
                    if (itemToRemove != null)
                    {
                        CurrentTodoList.TodoItems.Remove(itemToRemove);
                    }
                }

                Console.WriteLine($"[INFO] Successfully deleted task ID: {item.Id}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error deleting task: {ex.Message}", "Error");
                Console.WriteLine($"[ERROR] Failed to delete task: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Filter and Sort Methods
        private bool FilterTodoItems(object obj)
        {
            if (obj is not TodoItemDto item)
                return false;

            // Apply Priority filter
            if (SelectedPriorityFilter.HasValue && item.Priority != SelectedPriorityFilter.Value)
                return false;

            // Apply Status filter
            if (SelectedStatusFilter.HasValue && item.Status != SelectedStatusFilter.Value)
                return false;

            // Apply Type filter
            if (SelectedTypeFilter.HasValue && item.TypeId != SelectedTypeFilter.Value)
                return false;

            return true;
        }

        private void ApplyFilter()
        {
            TodoItemsView?.Refresh();
        }

        private void ClearFilter()
        {
            SelectedPriorityFilter = null;
            SelectedStatusFilter = null;
            SelectedTypeFilter = null;
            // Filter will be applied automatically due to property setters
        }

        private void SortByColumn(string? columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                return;

            // Toggle sort direction if clicking the same column
            if (_sortColumn == columnName)
            {
                _isSortAscending = !_isSortAscending;
            }
            else
            {
                _sortColumn = columnName;
                _isSortAscending = true;
            }

            // Update public properties for UI binding
            CurrentSortColumn = _sortColumn;
            IsSortAscending = _isSortAscending;

            // Clear existing sort descriptions
            TodoItemsView.SortDescriptions.Clear();

            // Add new sort description
            var direction = _isSortAscending ? ListSortDirection.Ascending : ListSortDirection.Descending;
            
            switch (columnName.ToLower())
            {
                case "name":
                case "title":
                    TodoItemsView.SortDescriptions.Add(new SortDescription(nameof(TodoItemDto.Title), direction));
                    break;
                case "description":
                    TodoItemsView.SortDescriptions.Add(new SortDescription(nameof(TodoItemDto.Description), direction));
                    break;
                case "priority":
                    TodoItemsView.SortDescriptions.Add(new SortDescription(nameof(TodoItemDto.Priority), direction));
                    break;
                case "status":
                    TodoItemsView.SortDescriptions.Add(new SortDescription(nameof(TodoItemDto.Status), direction));
                    break;
                case "type":
                    TodoItemsView.SortDescriptions.Add(new SortDescription(nameof(TodoItemDto.TypeName), direction));
                    break;
                case "date":
                case "duedate":
                    TodoItemsView.SortDescriptions.Add(new SortDescription(nameof(TodoItemDto.DueDate), direction));
                    break;
            }
        }

        public void NavigateBack()
        {
            _navigationService.GoBack();
        }
    }
}
