using System;
using System.Windows;
using System.Windows.Controls;
using Jtodo.ViewModels;

namespace Jtodo.Views
{
    public partial class DetailPage : Page
    {
        private readonly DetailPageViewModel _viewModel;

        public DetailPage(string listId)
        {
            InitializeComponent();
            _viewModel = new DetailPageViewModel(App.NavigationService, App.TodoListService);
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
    }
}
