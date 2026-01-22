using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Jtodo.ViewModels;
using Jtodo.DTOs;

namespace Jtodo.Views
{
    public partial class DetailPage : Page
    {
        private readonly DetailPageViewModel _viewModel;

        public DetailPage(string listId)
        {
            InitializeComponent();
            _viewModel = new DetailPageViewModel(
                App.NavigationService, 
                App.TodoListService,
                App.TodoItemService,
                App.TypeService);
            DataContext = _viewModel;

            Loaded += async (s, e) =>
            {
                if (ulong.TryParse(listId, out ulong id))
                {
                    await _viewModel.LoadTodoListByIdAsync(id);
                }
                else
                {
                    MessageBox.Show($"Invalid Todo List ID: {listId}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.NavigateBack();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement menu functionality
        }

        private void DatePicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is DatePicker datePicker && datePicker.Tag is TodoItemDto item)
                {
                    // Execute the save command
                    if (_viewModel.SaveEditItemCommand.CanExecute(item))
                    {
                        _viewModel.SaveEditItemCommand.Execute(item);
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
