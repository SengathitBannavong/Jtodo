using System.Collections.ObjectModel;
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

        public ICommand NavigateToDetailCommand { get; }
        public ICommand CreateNewTodoListCommand { get; }

        public WelcomeViewModel(INavigationService navigationService, TodoListService todoListService)
        {
            _todoListService = todoListService;
            _navigationService = navigationService;
            TodoLists = new ObservableCollection<TodoListDto>();
            _displayItems = new ObservableCollection<object>();
            
            NavigateToDetailCommand = new RelayCommand(OnNavigateToDetail);
            CreateNewTodoListCommand = new RelayCommand(async p => await OnCreateNewTodoListAsync(p));
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
    }
}
