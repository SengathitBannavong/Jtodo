using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using Jtodo.Models;
using Jtodo.Domains;
using Jtodo.Commands;
using Jtodo.Values;

namespace Jtodo.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly HomeModel _model;
        private TodoItem? _selectedTodo;
        private TodoList? _selectedList;
        private ulong? _selectedListId;

        public ObservableCollection<TodoItem> TodoList => _model.TodoList;
        public ObservableCollection<TodoList> TodoLists => _model.TodoLists;

        // SelectedListId: UI binds ComboBox SelectedValue to this (SelectedValuePath="Id")
        public ulong? SelectedListId
        {
            get => _selectedListId;
            set
            {
                if (_selectedListId == value) return;
                _selectedListId = value;
                RaisePropertyChanged();

                if (value.HasValue)
                {
                    // load by id and update SelectedList reference
                    LoadListById(value.Value);
                    SelectedList = TodoLists.FirstOrDefault(t => t.Id == value.Value);
                }
                else
                {
                    SelectedList = null;
                }
            }
        }

        public TodoList? SelectedList
        {
            get => _selectedList;
            set
            {
                if (_selectedList == value) return;
                _selectedList = value;
                RaisePropertyChanged();
                OnSelectedListChanged();
            }
        }

        public TodoItem? SelectedTodo
        {
            get => _selectedTodo;
            set
            {
                if (_selectedTodo == value) return;
                _selectedTodo = value;
                RaisePropertyChanged();
            }
        }



        public HomeViewModel()
        {
            _model = new HomeModel();
            Load();
        }

        private void Load()
        {
            _model.Load_All_Todo_List();
            RaisePropertyChanged(nameof(TodoLists));

            // set default selection to first list if available
            if (TodoLists != null && TodoLists.Count > 0)
            {
                SelectedListId = TodoLists.First().Id;
            }

            RaisePropertyChanged(nameof(TodoList));
        }

        private void OnSelectedListChanged()
        {
            // when list changes, update TodoList collection to reflect selected list
            TodoList.Clear();
            if (SelectedList != null)
            {
                foreach (var item in SelectedList.Todo_Items)
                {
                    TodoList.Add(item);
                }
            }
        }

        public void LoadListById(ulong id)
        {
            _model.Load_Todo_List_ById(id);
            RaisePropertyChanged(nameof(TodoList));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
