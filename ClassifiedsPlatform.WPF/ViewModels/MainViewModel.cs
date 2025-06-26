using ClassifiedsPlatform.Domain;
using ClassifiedsPlatform.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ClassifiedsPlatform.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly string _filePath = "data.json";

        private List<User> _allUsers = new List<User>();
        private List<Ad> _allAds = new List<Ad>();
        private List<Category> _allCategories = new List<Category>();

        #region Public Properties for UI Binding

        private User? _currentUser;
        public User? CurrentUser
        {
            get => _currentUser;
            set { if (_currentUser != value) { _currentUser = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsUserLoggedIn)); OnPropertyChanged(nameof(IsUserAdmin)); FilterAds(); } }
        }

        public bool IsUserLoggedIn => CurrentUser != null;
        public bool IsUserAdmin => CurrentUser is Administrator;
        private string? _email;
        public string? Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        private string? _password;
        public string? Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        private string? _searchText;
        public string? SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); FilterAds(); } }
        private Category? _selectedFilterCategory;
        public Category? SelectedFilterCategory { get => _selectedFilterCategory; set { _selectedFilterCategory = value; OnPropertyChanged(); FilterAds(); } }

        public ObservableCollection<Category> FilterCategories { get; set; }
        public ObservableCollection<AdViewModel> DisplayedAds { get; set; }

        #endregion

        #region Commands
        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand OpenCreateAdWindowCommand { get; }
        public ICommand OpenAdminPanelCommand { get; }
        public ICommand DeleteAdCommand { get; }
        public ICommand EditAdCommand { get; }
        public ICommand OpenRegistrationWindowCommand { get; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            _dataService = new JsonDataService();
            DisplayedAds = new ObservableCollection<AdViewModel>();
            FilterCategories = new ObservableCollection<Category>();

            LoginCommand = new RelayCommand(Login);
            LogoutCommand = new RelayCommand(Logout, CanExecuteAuthCommands);
            OpenCreateAdWindowCommand = new RelayCommand(OpenCreateAdWindow, CanExecuteAuthCommands);
            OpenAdminPanelCommand = new RelayCommand(OpenAdminPanel, CanOpenAdminPanel);
            DeleteAdCommand = new RelayCommand(DeleteAd, CanExecuteAuthCommands);
            EditAdCommand = new RelayCommand(OpenEditAdWindow, CanExecuteAuthCommands);
            OpenRegistrationWindowCommand = new RelayCommand(OpenRegistrationWindow);

            LoadOrCreateData();
        }
        #endregion

        #region Command Methods

        private void FilterAds()
        {
            DisplayedAds.Clear();

            
            IEnumerable<Ad> tempFiltered = _allAds.Where(ad => ad.Status == AdStatus.Active);

            
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                tempFiltered = tempFiltered.Where(ad => ad.Title != null && ad.Title.ToLower().Contains(SearchText.ToLower()));
            }

            
            if (SelectedFilterCategory != null && SelectedFilterCategory.Id != 0)
            {
                tempFiltered = tempFiltered.Where(ad => ad.CategoryId == SelectedFilterCategory.Id);
            }

            foreach (var ad in tempFiltered)
            {
                var category = _allCategories.FirstOrDefault(c => c.Id == ad.CategoryId);
                var categoryName = category?.Name ?? "Не вказано";
                DisplayedAds.Add(new AdViewModel(ad, CurrentUser?.Id, categoryName));
            }
        }

        private void OpenAdminPanel(object? parameter)
        {
            
            var adminVM = new AdminViewModel(_allUsers, _allAds, _allCategories, _dataService, _filePath, FilterCategories);
            var adminWindow = new AdminWindow { DataContext = adminVM, Owner = Application.Current.MainWindow };
            adminWindow.ShowDialog();

            
            FilterAds();
        }

        
        private void Login(object? parameter)
        {
            
            var user = _allUsers.FirstOrDefault(u => u.Email == Email && u.Password == Password && u.Status == UserStatus.Active);

            if (user != null)
            {
                CurrentUser = user;
            }
            else
            {
                MessageBox.Show("Невірний email або пароль, або ваш акаунт заблоковано.");
            }
        }
        private void Logout(object? parameter) { CurrentUser = null; Email = string.Empty; Password = string.Empty; }
        private bool CanExecuteAuthCommands(object? parameter) => IsUserLoggedIn;
        private bool CanOpenAdminPanel(object? parameter) => IsUserAdmin;
        private void OpenRegistrationWindow(object? parameter) { var regVM = new RegistrationViewModel(_allUsers); var regWindow = new RegistrationWindow { DataContext = regVM, Owner = Application.Current.MainWindow }; if (regWindow.ShowDialog() == true && regVM.NewUser != null) { var newUser = regVM.NewUser; newUser.Id = (_allUsers.Any() ? _allUsers.Max(u => u.Id) : 0) + 1; _allUsers.Add(newUser); _dataService.SaveData(_allUsers, _allAds, _allCategories, _filePath); CurrentUser = newUser; MessageBox.Show("Реєстрація успішна! Ви увійшли до системи.", "Вітаємо!"); } }
        private void OpenCreateAdWindow(object? parameter) { var createAdVM = new CreateAdViewModel(_allCategories); var adWindow = new CreateAdWindow { DataContext = createAdVM, Owner = Application.Current.MainWindow }; if (adWindow.ShowDialog() == true && createAdVM.AdToProcess != null && CurrentUser != null) { var newAd = createAdVM.AdToProcess; newAd.Id = (_allAds.Any() ? _allAds.Max(a => a.Id) : 0) + 1; newAd.AuthorId = CurrentUser.Id; newAd.CreationDate = DateTime.Now; _allAds.Add(newAd); _dataService.SaveData(_allUsers, _allAds, _allCategories, _filePath); FilterAds(); MessageBox.Show("Оголошення успішно створено і відправлено на модерацію!"); } }
        private void OpenEditAdWindow(object? parameter) { if (parameter is AdViewModel adVMToEdit) { var editAdVM = new CreateAdViewModel(_allCategories, adVMToEdit.Ad); var adWindow = new CreateAdWindow { DataContext = editAdVM, Owner = Application.Current.MainWindow, Title = "Редагувати оголошення" }; if (adWindow.ShowDialog() == true) { _dataService.SaveData(_allUsers, _allAds, _allCategories, _filePath); FilterAds(); MessageBox.Show("Оголошення успішно оновлено!"); } } }
        private void DeleteAd(object? parameter) { if (parameter is AdViewModel adVMToDelete) { var result = MessageBox.Show($"Ви впевнені, що хочете видалити оголошення '{adVMToDelete.Title}'?", "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Warning); if (result == MessageBoxResult.Yes) { var adInMasterList = _allAds.FirstOrDefault(a => a.Id == adVMToDelete.Id); if (adInMasterList != null) { _allAds.Remove(adInMasterList); _dataService.SaveData(_allUsers, _allAds, _allCategories, _filePath); FilterAds(); } } } }

        #endregion

        #region Helper Methods
        private void LoadOrCreateData()
        {
            if (File.Exists(_filePath))
            {
                var (users, ads, categories) = _dataService.LoadData(_filePath);
                _allUsers = new List<User>(users);
                _allAds = new List<Ad>(ads);
                _allCategories = new List<Category>(categories);
            }
            else
            {
                _allUsers = new List<User> { new RegisteredUser { Id = 1, Email = "user@test.com", Password = "123", Name = "John Doe", Status = UserStatus.Active }, new Administrator { Id = 2, Email = "admin@test.com", Password = "admin", Status = UserStatus.Active } };
                _allCategories = new List<Category> { new Category { Id = 1, Name = "Електроніка" }, new Category { Id = 2, Name = "Нерухомість" } };
                _allAds = new List<Ad> { new Ad { Id = 1, Title = "Продам ноутбук", Description = "Потужний ігровий ноутбук", Price = 25000, AuthorId = 1, CategoryId = 1, Status = AdStatus.Active, CreationDate = DateTime.Now.AddDays(-2) }, new Ad { Id = 2, Title = "Оренда квартири", Description = "2-кімнатна квартира в центрі", Price = 8000, AuthorId = 1, CategoryId = 2, Status = AdStatus.Active, CreationDate = DateTime.Now.AddDays(-1) }, new Ad { Id = 3, Title = "Телефон на запчастини", Description = "Екран розбитий, все інше працює", Price = 500, AuthorId = 1, CategoryId = 1, Status = AdStatus.Active, CreationDate = DateTime.Now } };
                _dataService.SaveData(_allUsers, _allAds, _allCategories, _filePath);
            }

            FilterCategories.Clear();
            FilterCategories.Add(new Category { Id = 0, Name = "Всі категорії" });
            foreach (var category in _allCategories)
            {
                FilterCategories.Add(category);
            }
            FilterAds();
        }
        #endregion
    }
}