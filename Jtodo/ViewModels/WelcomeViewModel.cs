using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Jtodo.Commands;
using Jtodo.Domains;
using Jtodo.DTOs;
using Jtodo.Interfaces;
using Jtodo.Services;

namespace Jtodo.ViewModels
{
    public class WelcomeViewModel : ViewModelBase
    {
        private readonly TodoListService _todoListService;
        private readonly INavigationService _navigationService;
        private ObservableCollection<object> _displayItems;
        private bool _isEditMode;

        public ObservableCollection<TodoListDto> TodoLists { get; set; }
        
        public ObservableCollection<object> DisplayItems
        {
            get => _displayItems;
            set
            {
                _displayItems = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToDetailCommand { get; }
        public ICommand CreateNewTodoListCommand { get; }
        public ICommand ToggleEditModeCommand { get; }
        public ICommand ConfirmDeleteCommand { get; }

        public WelcomeViewModel(INavigationService navigationService, TodoListService todoListService)
        {
            _todoListService = todoListService;
            _navigationService = navigationService;
            TodoLists = new ObservableCollection<TodoListDto>();
            _displayItems = new ObservableCollection<object>();
            
            NavigateToDetailCommand = new RelayCommand(OnNavigateToDetail);
            CreateNewTodoListCommand = new RelayCommand(async p => await OnCreateNewTodoListAsync(p));
            ToggleEditModeCommand = new RelayCommand(p => ToggleEditMode());
            ConfirmDeleteCommand = new RelayCommand(async p => await OnConfirmDeleteAsync(p));
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
                LoadingMessage = "Loading Todo List...";

                var dtos = await _todoListService.Get_All_Todo_list_Async();
                
                TodoLists.Clear();
                foreach (var dto in dtos)
                {
                    TodoLists.Add(dto);
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
            foreach (var dto in TodoLists)
            {
                DisplayItems.Add(dto);
            }
        }

        private void OnNavigateToDetail(object? parameter)
        {
            if (parameter is TodoListDto dto)
            {
                _navigationService.NavigateToDetail(dto.Id.ToString());
            }
            else if (parameter is TodoList domain)
            {
                _navigationService.NavigateToDetail(domain.Id.ToString());
            }
        }

        private async Task OnCreateNewTodoListAsync(object? parameter)
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading New Data...";

                var newDto = new TodoListDto(
                    title: "Mock",
                    description: "Mock"
                );
                var generatedId = await _todoListService.Create_Todo_List_Async(newDto);
                
                System.Console.WriteLine($"[INFO] Created new TodoList with ID: {generatedId}");
                await LoadTodoListsAsync();

                System.Windows.MessageBox.Show(
                    $"New Todo List Creat Compled!\nID: {generatedId}",
                    "Compled",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error to create: {ex.Message}",
                    "Invalid",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
            System.Console.WriteLine($"[INFO] Edit Mode: {IsEditMode}");
        }

        private async Task OnConfirmDeleteAsync(object? parameter)
        {
            if (parameter is not TodoListDto dto)
                return;

            try
            {
                // 1. Get First 10 words of Title
                string first10Words = GetFirst10Words(dto.Title);

                // 2. Show Confirmation Dialog
                var confirmWindow = new Views.DeleteConfirmationWindow(dto.Title, first10Words);
                bool? result = confirmWindow.ShowDialog();

                if (result == true)
                {
                    // 3. Remove TodoList
                    IsLoading = true;
                    LoadingMessage = "Deleting...";

                    await _todoListService.Delete_TodoList_Complete_Async(dto.Id);
                    
                    System.Console.WriteLine($"[INFO] Deleted TodoList ID: {dto.Id}");

                    // 4. Reload lists
                    await LoadTodoListsAsync();

                    System.Windows.MessageBox.Show(
                        "Delete Completed!",
                        "Completed",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
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

        private string GetFirst10Words(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            var first10 = words.Take(10);
            return string.Join(" ", first10);
        }
    }
}

