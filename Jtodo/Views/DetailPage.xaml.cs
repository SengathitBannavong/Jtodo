using System;
using System.Windows;
using System.Windows.Controls;
using Jtodo.ViewModels;
using Jtodo.Services.Interfaces;
using Jtodo.Services;

namespace Jtodo.Views
{
    public partial class DetailPage : Page
    {
        private readonly DetailPageViewModel _viewModel;
        private readonly INavigationService _navigationService;

        public DetailPage(string listId)
        {
            InitializeComponent();
            _navigationService = new AppNavigationService();
            _viewModel = new DetailPageViewModel(_navigationService);
            DataContext = _viewModel;

            if (ulong.TryParse(listId, out ulong id))
            {
                _viewModel.LoadTodoListById(id);
            }
            else
            {
                MessageBox.Show($"Invalid Todo List ID: {listId}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Loaded += (s, e) =>
            {
                if (_navigationService is AppNavigationService navService && NavigationService != null)
                {
                    navService.SetNavigationService(NavigationService);
                }
            };
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.NavigateBack();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
