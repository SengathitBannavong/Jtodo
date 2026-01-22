using Jtodo.ViewModels;
using System.ComponentModel;

namespace Jtodo.DTOs
{
    public class TypeItemViewModel : ViewModelBase
    {
        private ulong _id;
        private string _text;
        private string _colorHex;
        private bool _isEditing;
        private string _editingText;
        private string _editingColorHex;

        public ulong Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(nameof(Text)); }
        }

        public string ColorHex
        {
            get => _colorHex;
            set { _colorHex = value; OnPropertyChanged(nameof(ColorHex)); }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(nameof(IsEditing)); }
        }

        public string EditingText
        {
            get => _editingText;
            set { _editingText = value; OnPropertyChanged(nameof(EditingText)); }
        }

        public string EditingColorHex
        {
            get => _editingColorHex;
            set { _editingColorHex = value; OnPropertyChanged(nameof(EditingColorHex)); }
        }

        // Property to check if this is the default "None" type
        public bool IsNoneType => Text == "None";

        public TypeItemViewModel()
        {
            _text = string.Empty;
            _colorHex = "#9E9E9E";
            _editingText = string.Empty;
            _editingColorHex = "#9E9E9E";
        }

        public TypeItemViewModel(ulong id, string text, int colorInt)
        {
            _id = id;
            _text = text;
            _colorHex = ConvertIntToHex(colorInt);
            _editingText = text;
            _editingColorHex = _colorHex;
        }

        private string ConvertIntToHex(int colorInt)
        {
            // Convert integer to hex color (assuming RGB format)
            return $"#{colorInt:X6}";
        }

        public int GetColorAsInt()
        {
            // Convert hex color to integer
            if (ColorHex.StartsWith("#"))
            {
                var hex = ColorHex.Substring(1);
                if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int result))
                {
                    return result;
                }
            }
            return 0x9E9E9E; // Default gray
        }
    }
}
