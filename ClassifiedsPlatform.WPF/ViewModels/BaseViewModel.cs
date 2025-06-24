using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClassifiedsPlatform.WPF.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        // Подія, яка повідомляє UI про зміну властивості
        public event PropertyChangedEventHandler? PropertyChanged;

        // Метод для виклику цієї події
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}