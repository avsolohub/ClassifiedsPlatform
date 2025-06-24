using ClassifiedsPlatform.Domain;
using ClassifiedsPlatform.WPF.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ClassifiedsPlatform.WPF.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly string _filePath;

        // Посилання на оригінальні списки з MainViewModel
        private readonly List<User> _allUsers;
        private readonly List<Ad> _allAds;
        private readonly List<Category> _allCategories;

        public ObservableCollection<UserViewModel> Users { get; set; }
        public ICommand BlockUserCommand { get; }

        
        public AdminViewModel(List<User> allUsers, List<Ad> allAds, List<Category> allCategories, IDataService dataService, string filePath)
        {
            _allUsers = allUsers;
            _allAds = allAds; // Зберігаємо посилання на список оголошень
            _allCategories = allCategories; // Зберігаємо посилання на список категорій
            _dataService = dataService;
            _filePath = filePath;

            Users = new ObservableCollection<UserViewModel>(
                allUsers.Select(u => new UserViewModel(u))
            );

            BlockUserCommand = new RelayCommand(BlockUser, CanBlockUser);
        }

        private bool CanBlockUser(object? parameter)
        {
            if (parameter is UserViewModel userVM)
            {
                return userVM.CanBeBlocked;
            }
            return false;
        }

        private void BlockUser(object? parameter)
        {
            if (parameter is UserViewModel userVM)
            {
                userVM.User.Status = UserStatus.Blocked;

                
                _dataService.SaveData(_allUsers, _allAds, _allCategories, _filePath);

                userVM.Refresh();
            }
        }
    }

    // Вкладений клас UserViewModel залишається без змін
    public class UserViewModel : BaseViewModel
    {
        public User User { get; }
        public int Id => User.Id;
        public string? Email => User.Email;
        public UserStatus Status => User.Status;
        public string? Name => (User as RegisteredUser)?.Name;

        public bool CanBeBlocked => !(User is Administrator) && User.Status == UserStatus.Active;

        public UserViewModel(User user)
        {
            User = user;
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(CanBeBlocked));
        }
    }
}