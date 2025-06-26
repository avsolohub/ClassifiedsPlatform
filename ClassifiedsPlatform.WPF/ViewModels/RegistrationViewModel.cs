using ClassifiedsPlatform.Domain;
using ClassifiedsPlatform.WPF.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ClassifiedsPlatform.WPF.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
        private readonly IEnumerable<User> _existingUsers;

        public string? Name { get; set; } 
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        public RegisteredUser? NewUser { get; private set; }

        public ICommand RegisterCommand { get; }

        public RegistrationViewModel(IEnumerable<User> existingUsers)
        {
            _existingUsers = existingUsers;
            RegisterCommand = new RelayCommand(Register, CanRegister);
        }

        private bool CanRegister(object? parameter)
        {
            
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        private void Register(object? parameter)
        {
            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Паролі не співпадають.", "Помилка реєстрації");
                return;
            }
            if (_existingUsers.Any(u => u.Email?.ToLower() == Email?.ToLower()))
            {
                MessageBox.Show("Користувач з таким email вже існує.", "Помилка реєстрації");
                return;
            }

            NewUser = new RegisteredUser
            {
                Name = this.Name, 
                Email = this.Email,
                Password = this.Password,
                Status = UserStatus.Active
            };

            CloseWindow(true);
        }

        private void CloseWindow(bool dialogResult)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = dialogResult;
                    window.Close();
                    break;
                }
            }
        }
    }
}