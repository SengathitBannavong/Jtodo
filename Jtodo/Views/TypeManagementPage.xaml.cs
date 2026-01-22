using System.Windows.Controls;
using Jtodo.ViewModels;

namespace Jtodo.Views
{
    public partial class TypeManagementPage : Page
    {
        private readonly TypeManagementViewModel _viewModel;

        public TypeManagementPage()
        {
            InitializeComponent();
            _viewModel = new TypeManagementViewModel(App.TypeService);
            DataContext = _viewModel;

            Loaded += async (s, e) =>
            {
                await _viewModel.LoadTypesAsync();
            };
        }
    }
}
