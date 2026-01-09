using Jtodo.Domains;
using Jtodo.Services;
using Jtodo.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jtodo.ViewModels
{
    public class DetailPageViewModel : INotifyPropertyChanged
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
                RaisePropertyChanged();
            }
        }

        public string ListTitle
        {
            get => _listTitle;
            set
            {
                _listTitle = value;
                RaisePropertyChanged();
            }
        }

        public string ListDescription
        {
            get => _listDescription;
            set
            {
                _listDescription = value;
                RaisePropertyChanged();
            }
        }

        public ulong ListId
        {
            get => _listId;
            set
            {
                _listId = value;
                RaisePropertyChanged();
            }
        }

        public TodoList? CurrentTodoList
        {
            get => _currentTodoList;
            private set
            {
                _currentTodoList = value;
                RaisePropertyChanged();
            }
        }

        public DetailPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _todoListService = new TodoListService();
            _todoItems = new ObservableCollection<TodoItem>();
            _listTitle = string.Empty;
            _listDescription = string.Empty;
        }

        public void LoadTodoListById(ulong todoListId)
        {
            CurrentTodoList = _todoListService.Get_Todo_List(todoListId);

            if (CurrentTodoList != null)
            {
                ListId = CurrentTodoList.Id;
                ListTitle = CurrentTodoList.Title;
                ListDescription = CurrentTodoList.Description;

                // Load todo items
                TodoItems.Clear();
                foreach (var item in CurrentTodoList.Todo_Items)
                {
                    TodoItems.Add(item);
                }
            }
            else
            {
                ListTitle = "Todo List Not Found";
                ListDescription = string.Empty;
                TodoItems.Clear();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        internal void NavigateBack()
        {
           _navigationService.GoBack();
        }
    }
}
