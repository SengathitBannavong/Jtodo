using System.Windows;

namespace Jtodo.Views
{
    public partial class DeleteConfirmationWindow : Window
    {
        private readonly string _requiredText;

        public DeleteConfirmationWindow(string fullTitle, string first10Words)
        {
            InitializeComponent();

            // Set title and required text
            TitleTextBlock.Text = fullTitle;
            RequiredTextBlock.Text = first10Words;
            _requiredText = first10Words;

            // Focus on TextBox
            Loaded += (s, e) => ConfirmTextBox.Focus();
        }

        private void ConfirmTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Enable Delete button only if text matches
            DeleteButton.IsEnabled = ConfirmTextBox.Text == _requiredText;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Return true (confirm delete)
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Return false (cancel delete)
            DialogResult = false;
            Close();
        }
    }
}
