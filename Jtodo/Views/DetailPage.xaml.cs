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

        private void TitleTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Auto-focus when TextBox is loaded in edit mode
            if (sender is TextBox textBox && textBox.DataContext is TodoItemDto item)
            {
                if (item.IsEditing && string.IsNullOrEmpty(item.Title))
                {
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private async void EditControl_LostFocus(object sender, RoutedEventArgs e)
        {
            // Auto-save when edit control loses focus
            if (sender is FrameworkElement element && element.Tag is TodoItemDto item)
            {
                if (item.IsEditing && _viewModel.SaveEditItemCommand.CanExecute(item))
                {
                    await System.Threading.Tasks.Task.Run(() => 
                    {
                        Dispatcher.Invoke(() => _viewModel.SaveEditItemCommand.Execute(item));
                    });
                }
            }
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
