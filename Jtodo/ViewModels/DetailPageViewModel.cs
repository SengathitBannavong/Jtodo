using Jtodo.Commands;
using Jtodo.DTOs;
using Jtodo.Interfaces;
using Jtodo.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Jtodo.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
        private readonly TodoListService _todoListService;
        private readonly INavigationService _navigationService;
        private TodoListDto? _currentTodoList;
        private ObservableCollection<TodoItemDto> _todoItems;
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

        public DetailPageViewModel(INavigationService navigationService, TodoListService todoListService)
        {
            _navigationService = navigationService;
            _todoListService = todoListService;
            _todoItems = new ObservableCollection<TodoItemDto>();
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
                    if (todoListDto.TodoItems != null)
                    {
                        foreach (var item in todoListDto.TodoItems)
                            TodoItems.Add(item);
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

        public void NavigateBack()
        {
            _navigationService.GoBack();
        }
    }
}