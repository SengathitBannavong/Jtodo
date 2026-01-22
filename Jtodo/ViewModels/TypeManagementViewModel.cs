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

        public TypeManagementViewModel(TypeService typeService)
        {
            _typeService = typeService;
            _typeItems = new ObservableCollection<TypeItemViewModel>();

            StartEditTypeCommand = new RelayCommand(p => StartEditType(p as TypeItemViewModel));
            SaveTypeCommand = new RelayCommand(async p => await SaveTypeAsync(p as TypeItemViewModel));
            CancelEditTypeCommand = new RelayCommand(p => CancelEditType(p as TypeItemViewModel));
            DeleteTypeCommand = new RelayCommand(async p => await DeleteTypeAsync(p as TypeItemViewModel));
            AddNewTypeCommand = new RelayCommand(async p => await AddNewTypeAsync());
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
