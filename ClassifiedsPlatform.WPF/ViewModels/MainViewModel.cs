using ClassifiedsPlatform.Domain;
using ClassifiedsPlatform.WPF.Commands;
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

        private string? _email;
        public string? Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private string? _password;
        public string? Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        private User? _currentUser;
        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsUserLoggedIn));
                    OnPropertyChanged(nameof(IsUserAdmin)); // Повідомляємо про зміну статусу адміна
                }
            }
        }

        public bool IsUserLoggedIn => CurrentUser != null;
        public bool IsUserAdmin => CurrentUser is Administrator; // Властивість для перевірки ролі адміна

        private string? _searchText;
        public string? SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); FilterAds(); } }

        private ObservableCollection<Ad> _displayedAds;
        public ObservableCollection<Ad> DisplayedAds { get => _displayedAds; set { _displayedAds = value; OnPropertyChanged(); } }

        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand OpenCreateAdWindowCommand { get; }
        public ICommand OpenAdminPanelCommand { get; } // Команда для адмін-панелі

        public MainViewModel()
        {
            _dataService = new JsonDataService();
            _displayedAds = new ObservableCollection<Ad>();

            LoginCommand = new RelayCommand(Login);
            LogoutCommand = new RelayCommand(Logout, CanExecuteAuthCommands);
            OpenCreateAdWindowCommand = new RelayCommand(OpenCreateAdWindow, CanExecuteAuthCommands);
            OpenAdminPanelCommand = new RelayCommand(OpenAdminPanel, CanOpenAdminPanel); // Ініціалізація нової команди

            LoadOrCreateData();
        }

        private void Login(object? parameter)
        {
            var user = _allUsers.FirstOrDefault(u => u.Email == Email && u.Password == Password);
            if (user != null)
            {
                CurrentUser = user;
            }
            else
            {
                MessageBox.Show("Невірний email або пароль.");
            }
        }

        private void Logout(object? parameter)
        {
            CurrentUser = null;
            Email = string.Empty;
            Password = string.Empty;
        }

        private void OpenCreateAdWindow(object? parameter)
        {
            var createAdVM = new CreateAdViewModel(_allCategories);
            var createAdWindow = new CreateAdWindow { DataContext = createAdVM, Owner = Application.Current.MainWindow };

            if (createAdWindow.ShowDialog() == true && createAdVM.NewAd != null && CurrentUser != null)
            {
                var newAd = createAdVM.NewAd;
                newAd.Id = (_allAds.Any() ? _allAds.Max(a => a.Id) : 0) + 1;
                newAd.AuthorId = CurrentUser.Id;
                newAd.CreationDate = System.DateTime.Now;
                _allAds.Add(newAd);
                _dataService.SaveData(_allUsers, _allAds, _allCategories, _filePath);
                FilterAds();
                MessageBox.Show("Оголошення успішно створено і відправлено на модерацію!");
            }
        }

        private bool CanExecuteAuthCommands(object? parameter) => IsUserLoggedIn;

        private bool CanOpenAdminPanel(object? parameter) => IsUserAdmin;

        private void OpenAdminPanel(object? parameter)
        {
            
            var adminVM = new AdminViewModel(_allUsers, _allAds, _allCategories, _dataService, _filePath);

            var adminWindow = new AdminWindow
            {
                DataContext = adminVM,
                Owner = Application.Current.MainWindow
            };
            adminWindow.ShowDialog();

            // Після закриття адмін-панелі оновлюємо список оголошень на випадок змін
            FilterAds();
        }

        private void FilterAds()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchText) ? _allAds : _allAds.Where(ad => ad.Title != null && ad.Title.ToLower().Contains(SearchText.ToLower()));
            DisplayedAds = new ObservableCollection<Ad>(filtered);
        }

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
                _allUsers = new List<User>
                {
                    new RegisteredUser { Id = 1, Email = "user@test.com", Password = "123", Name = "John Doe", Status = UserStatus.Active },
                    new Administrator { Id = 2, Email = "admin@test.com", Password = "admin", Status = UserStatus.Active }
                };
                _allCategories = new List<Category> { new Category { Id = 1, Name = "Електроніка" }, new Category { Id = 2, Name = "Нерухомість" } };
                _allAds = new List<Ad>
{
    new Ad { Id = 1, Title = "Продам ноутбук", Description = "Потужний ігровий ноутбук", Price = 25000, AuthorId = 1, CategoryId = 1, Status = AdStatus.Active, CreationDate = System.DateTime.Now.AddDays(-2) },
    new Ad { Id = 2, Title = "Оренда квартири", Description = "2-кімнатна квартира в центрі", Price = 8000, AuthorId = 1, CategoryId = 2, Status = AdStatus.Active, CreationDate = System.DateTime.Now.AddDays(-1) },
    new Ad { Id = 3, Title = "Телефон на запчастини", Description = "Екран розбитий, все інше працює", Price = 500, AuthorId = 1, CategoryId = 1, Status = AdStatus.Active, CreationDate = System.DateTime.Now }
};
                _dataService.SaveData(_allUsers, _allAds, _allCategories, _filePath);
            }
            FilterAds();
        }
    }
}