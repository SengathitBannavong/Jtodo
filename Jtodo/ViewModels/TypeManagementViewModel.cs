using Jtodo.Commands;
using Jtodo.DTOs;
using Jtodo.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Jtodo.ViewModels
{
    public class TypeManagementViewModel : ViewModelBase
    {
        private readonly TypeService _typeService;
        private ObservableCollection<TypeItemViewModel> _typeItems;

        public ObservableCollection<TypeItemViewModel> TypeItems
        {
            get => _typeItems;
            set { _typeItems = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand StartEditTypeCommand { get; }
        public ICommand SaveTypeCommand { get; }
        public ICommand CancelEditTypeCommand { get; }
        public ICommand DeleteTypeCommand { get; }
        public ICommand AddNewTypeCommand { get; }
        public ICommand SelectColorCommand { get; }

        public TypeManagementViewModel(TypeService typeService)
        {
            _typeService = typeService;
            _typeItems = new ObservableCollection<TypeItemViewModel>();

            StartEditTypeCommand = new RelayCommand(p => StartEditType(p as TypeItemViewModel));
            SaveTypeCommand = new RelayCommand(async p => await SaveTypeAsync(p as TypeItemViewModel));
            CancelEditTypeCommand = new RelayCommand(p => CancelEditType(p as TypeItemViewModel));
            DeleteTypeCommand = new RelayCommand(async p => await DeleteTypeAsync(p as TypeItemViewModel));
            AddNewTypeCommand = new RelayCommand(async p => await AddNewTypeAsync());
            SelectColorCommand = new RelayCommand(p => SelectColor(p as TypeItemViewModel));
        }

        public async Task LoadTypesAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading types...";

                var types = await _typeService.GetAllTypesAsync();
                TypeItems.Clear();

                foreach (var type in types)
                {
                    TypeItems.Add(new TypeItemViewModel(type.Id, type.Text, type.Color));
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading types: {ex.Message}", "Error");
                Console.WriteLine($"[ERROR] Failed to load types: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void StartEditType(TypeItemViewModel? item)
        {
            if (item == null) return;
            
            item.EditingText = item.Text;
            item.EditingColorHex = item.ColorHex;
            item.IsEditing = true;
        }

        private async Task SaveTypeAsync(TypeItemViewModel? item)
        {
            if (item == null) return;

            // Validate
            if (string.IsNullOrWhiteSpace(item.EditingText))
            {
                System.Windows.MessageBox.Show("Type name cannot be empty", "Validation Error");
                return;
            }

            if (!IsValidHexColor(item.EditingColorHex))
            {
                System.Windows.MessageBox.Show("Invalid color format. Use #RRGGBB format", "Validation Error");
                return;
            }

            try
            {
                IsLoading = true;
                LoadingMessage = "Saving type...";

                // Update the domain object
                var type = new Domains.Type(item.Id, item.EditingText, HexToInt(item.EditingColorHex));
                await _typeService.UpdateTypeAsync(type);

                // Update the view model
                item.Text = item.EditingText;
                item.ColorHex = item.EditingColorHex;
                item.IsEditing = false;

                Console.WriteLine($"[INFO] Successfully saved type ID: {item.Id}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving type: {ex.Message}", "Error");
                Console.WriteLine($"[ERROR] Failed to save type: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CancelEditType(TypeItemViewModel? item)
        {
            if (item == null) return;
            
            item.EditingText = item.Text;
            item.EditingColorHex = item.ColorHex;
            item.IsEditing = false;
        }

        private async Task DeleteTypeAsync(TypeItemViewModel? item)
        {
            if (item == null) return;

            // Protect "None" type from deletion
            if (item.Text == "None")
            {
                System.Windows.MessageBox.Show(
                    "Cannot delete the default 'None' type.\nThis type is required by the system.",
                    "Cannot Delete",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                var result = System.Windows.MessageBox.Show(
                    $"Are you sure you want to delete type '{item.Text}'?",
                    "Confirm Delete",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (result != System.Windows.MessageBoxResult.Yes)
                    return;

                IsLoading = true;
                LoadingMessage = "Deleting type...";

                await _typeService.DeleteTypeAsync(item.Id);
                TypeItems.Remove(item);

                Console.WriteLine($"[INFO] Successfully deleted type ID: {item.Id}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error deleting type: {ex.Message}", "Error");
                Console.WriteLine($"[ERROR] Failed to delete type: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddNewTypeAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Adding new type...";

                // Create new type with default values
                var newType = new Domains.Type(0, "New Type", 0x9E9E9E);
                var createdType = await _typeService.CreateTypeAsync(newType);

                // Add to UI
                var newItem = new TypeItemViewModel(createdType.Id, createdType.Text, createdType.Color)
                {
                    IsEditing = true,
                    EditingText = ""
                };

                TypeItems.Add(newItem);

                Console.WriteLine($"[INFO] Successfully added new type");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error adding type: {ex.Message}", "Error");
                Console.WriteLine($"[ERROR] Failed to add type: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void SelectColor(TypeItemViewModel? item)
        {
            if (item == null) return;

            try
            {
                // Create a simple WPF window for color selection
                var colorPickerWindow = new System.Windows.Window
                {
                    Title = "Select Color",
                    Width = 400,
                    Height = 300,
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                    ResizeMode = System.Windows.ResizeMode.NoResize
                };

                var stackPanel = new System.Windows.Controls.StackPanel
                {
                    Margin = new System.Windows.Thickness(20)
                };

                // Color sliders
                var redSlider = CreateColorSlider("Red");
                var greenSlider = CreateColorSlider("Green");
                var blueSlider = CreateColorSlider("Blue");

                // Preview
                var preview = new System.Windows.Controls.Border
                {
                    Height = 60,
                    Margin = new System.Windows.Thickness(0, 10, 0, 10),
                    CornerRadius = new System.Windows.CornerRadius(5)
                };

                // Buttons
                var buttonPanel = new System.Windows.Controls.StackPanel
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                };

                var okButton = new System.Windows.Controls.Button
                {
                    Content = "OK",
                    Width = 80,
                    Height = 30,
                    Margin = new System.Windows.Thickness(5)
                };

                var cancelButton = new System.Windows.Controls.Button
                {
                    Content = "Cancel",
                    Width = 80,
                    Height = 30,
                    Margin = new System.Windows.Thickness(5)
                };

                bool? result = null;

                okButton.Click += (s, e) =>
                {
                    result = true;
                    colorPickerWindow.Close();
                };

                cancelButton.Click += (s, e) =>
                {
                    result = false;
                    colorPickerWindow.Close();
                };

                System.Windows.RoutedPropertyChangedEventHandler<double> updatePreview = (s, e) =>
                {
                    var color = System.Windows.Media.Color.FromRgb(
                        (byte)redSlider.Value,
                        (byte)greenSlider.Value,
                        (byte)blueSlider.Value);
                    preview.Background = new System.Windows.Media.SolidColorBrush(color);
                };

                redSlider.ValueChanged += updatePreview;
                greenSlider.ValueChanged += updatePreview;
                blueSlider.ValueChanged += updatePreview;

                // Set initial values
                try
                {
                    var currentColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(item.ColorHex);
                    redSlider.Value = currentColor.R;
                    greenSlider.Value = currentColor.G;
                    blueSlider.Value = currentColor.B;
                }
                catch
                {
                    redSlider.Value = 158;
                    greenSlider.Value = 158;
                    blueSlider.Value = 158;
                }

                buttonPanel.Children.Add(okButton);
                buttonPanel.Children.Add(cancelButton);

                stackPanel.Children.Add(redSlider);
                stackPanel.Children.Add(greenSlider);
                stackPanel.Children.Add(blueSlider);
                stackPanel.Children.Add(preview);
                stackPanel.Children.Add(buttonPanel);

                colorPickerWindow.Content = stackPanel;
                colorPickerWindow.ShowDialog();

                if (result == true)
                {
                    var selectedColor = System.Windows.Media.Color.FromRgb(
                        (byte)redSlider.Value,
                        (byte)greenSlider.Value,
                        (byte)blueSlider.Value);
                    
                    var hexColor = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                    
                    item.ColorHex = hexColor;
                    
                    if (item.IsEditing)
                    {
                        item.EditingColorHex = hexColor;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error selecting color: {ex.Message}", "Error");
                Console.WriteLine($"[ERROR] Failed to select color: {ex.Message}");
            }
        }

        private System.Windows.Controls.Slider CreateColorSlider(string label)
        {
            var panel = new System.Windows.Controls.StackPanel();
            var textBlock = new System.Windows.Controls.TextBlock
            {
                Text = label,
                Margin = new System.Windows.Thickness(0, 5, 0, 5)
            };
            
            var slider = new System.Windows.Controls.Slider
            {
                Minimum = 0,
                Maximum = 255,
                Value = 128,
                TickFrequency = 1,
                IsSnapToTickEnabled = true
            };

            return slider;
        }

        private bool IsValidHexColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color)) return false;
            if (!color.StartsWith("#")) return false;
            if (color.Length != 7) return false;

            var hex = color.Substring(1);
            return int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out _);
        }

        private int HexToInt(string hexColor)
        {
            if (hexColor.StartsWith("#"))
            {
                var hex = hexColor.Substring(1);
                if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int result))
                {
                    return result;
                }
            }
            return 0x9E9E9E;
        }
    }
}
