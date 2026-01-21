using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Jtodo.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isLoading;
        private string _loadingMessage;

        public bool IsLoading
        {
            get => _isLoading;
            protected set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LoadingMessage
        {
            get => _loadingMessage;
            protected set
            {
                if (_loadingMessage != value)
                {
                    _loadingMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        protected ViewModelBase()
        {
            _loadingMessage = "Loading...";
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
