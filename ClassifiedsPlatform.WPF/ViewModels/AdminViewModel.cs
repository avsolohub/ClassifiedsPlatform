using ClassifiedsPlatform.Domain;
using ClassifiedsPlatform.WPF.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ClassifiedsPlatform.WPF.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly string _filePath;

        private readonly List<User> _allUsers;
        private readonly List<Ad> _allAds;
        private readonly List<Category> _allCategories;
        private readonly ObservableCollection<Category> _filterCategories;

        public ObservableCollection<UserViewModel> Users { get; set; }
        public ObservableCollection<Ad> PendingAds { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        private string? _newCategoryName;
        public string? NewCategoryName { get => _newCategoryName; set { _newCategoryName = value; OnPropertyChanged(); } }

        // --- Команди ---
        public ICommand ToggleBlockStatusCommand { get; } 
        public ICommand DeleteUserCommand { get; }
        public ICommand ApproveAdCommand { get; }
        public ICommand RejectAdCommand { get; }
        public ICommand AddCategoryCommand { get; }

        public AdminViewModel(List<User> allUsers, List<Ad> allAds, List<Category> allCategories, IDataService dataService, string filePath, ObservableCollection<Category> filterCategories)
        {
            _allUsers = allUsers;
            _allAds = allAds;
            _allCategories = allCategories;
            _filterCategories = filterCategories;
            _dataService = dataService;
            _filePath = filePath;

            Users = new ObservableCollection<UserViewModel>(allUsers.Select(u => new UserViewModel(u)));
            PendingAds = new ObservableCollection<Ad>(allAds.Where(a => a.Status == AdStatus.PendingModeration));
            Categories = new ObservableCollection<Category>(allCategories);

            ToggleBlockStatusCommand = new RelayCommand(ToggleBlockStatus, CanToggleBlockStatus); // Ініціалізація нової команди
            DeleteUserCommand = new RelayCommand(DeleteUser, CanDeleteUser);
            ApproveAdCommand = new RelayCommand(ApproveAd);
            RejectAdCommand = new RelayCommand(RejectAd);
            AddCategoryCommand = new RelayCommand(AddCategory, CanAddCategory);
        }

        #region User Management

        
        private void ToggleBlockStatus(object? parameter)
        {
            if (parameter is UserViewModel userVM)
            {
                userVM.User.Status = (userVM.User.Status == UserStatus.Active)
                    ? UserStatus.Blocked
                    : UserStatus.Active;

                SaveChanges();
                userVM.Refresh(); 
            }
        }

        
        private bool CanToggleBlockStatus(object? parameter) => parameter is UserViewModel userVM && !(userVM.User is Administrator);

        private bool CanDeleteUser(object? parameter) => parameter is UserViewModel userVM && userVM.CanBeDeleted;
        private void DeleteUser(object? parameter)
        {
            if (parameter is UserViewModel userVMToDelete)
            {
                var result = MessageBox.Show($"Ви впевнені, що хочете видалити користувача '{userVMToDelete.Email}'? Це також видалить всі його оголошення.", "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _allUsers.Remove(userVMToDelete.User);
                    _allAds.RemoveAll(ad => ad.AuthorId == userVMToDelete.Id);
                    Users.Remove(userVMToDelete);
                    SaveChanges();
                }
            }
        }
        #endregion

        #region Ad Moderation & Category Management
        private void ApproveAd(object? parameter) { if (parameter is Ad adToApprove) { adToApprove.Status = AdStatus.Active; PendingAds.Remove(adToApprove); SaveChanges(); MessageBox.Show($"Оголошення '{adToApprove.Title}' схвалено.", "Модерація"); } }
        private void RejectAd(object? parameter) { if (parameter is Ad adToReject) { adToReject.Status = AdStatus.Rejected; PendingAds.Remove(adToReject); SaveChanges(); MessageBox.Show($"Оголошення '{adToReject.Title}' відхилено.", "Модерація"); } }
        private bool CanAddCategory(object? parameter) => !string.IsNullOrWhiteSpace(NewCategoryName);
        private void AddCategory(object? parameter) { if (_allCategories.Any(c => c.Name?.ToLower() == NewCategoryName?.ToLower())) { MessageBox.Show("Категорія з такою назвою вже існує.", "Помилка"); return; } var newCategory = new Category { Id = (_allCategories.Any() ? _allCategories.Max(c => c.Id) : 0) + 1, Name = this.NewCategoryName }; _allCategories.Add(newCategory); Categories.Add(newCategory); _filterCategories.Add(newCategory); SaveChanges(); NewCategoryName = string.Empty; }
        #endregion

        private void SaveChanges()
        {
            _dataService.SaveData(_allUsers, _allAds, _allCategories, _filePath);
        }
    }

    
    public class UserViewModel : BaseViewModel
    {
        public User User { get; }
        public int Id => User.Id;
        public string? Email => User.Email;
        public UserStatus Status => User.Status;
        public string? Name => (User as RegisteredUser)?.Name;

        
        public string BlockUnblockButtonText => User.Status == UserStatus.Active ? "Блокувати" : "Розблокувати";

        public bool CanBeDeleted => !(User is Administrator);

        public UserViewModel(User user)
        {
            User = user;
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(BlockUnblockButtonText)); 
        }
    }
}