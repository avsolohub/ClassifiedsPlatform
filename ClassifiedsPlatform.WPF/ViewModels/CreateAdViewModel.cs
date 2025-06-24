using ClassifiedsPlatform.Domain;
using ClassifiedsPlatform.WPF.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ClassifiedsPlatform.WPF.ViewModels
{
    public class CreateAdViewModel : BaseViewModel
    {
        // Властивості для полів вводу
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Category? SelectedCategory { get; set; }

        // Властивість для списку доступних категорій
        public ObservableCollection<Category> AvailableCategories { get; set; }

        // Властивість для збереження результату
        public Ad? NewAd { get; private set; }

        public ICommand SaveCommand { get; }

        public CreateAdViewModel(IEnumerable<Category> categories)
        {
            AvailableCategories = new ObservableCollection<Category>(categories);
            SaveCommand = new RelayCommand(SaveChanges);
        }

        private void SaveChanges(object? parameter)
        {
            // Перевірка на заповнення даних
            if (string.IsNullOrWhiteSpace(Title) || SelectedCategory == null)
            {
                MessageBox.Show("Назва та категорія є обов'язковими.", "Помилка валідації");
                return;
            }

            // Створюємо нове оголошення
            NewAd = new Ad
            {
                Title = this.Title,
                Description = this.Description,
                Price = this.Price,
                CategoryId = this.SelectedCategory.Id,
                Status = AdStatus.PendingModeration // Нові оголошення йдуть на модерацію
            };

            // Закриваємо вікно, знайшовши його по DataContext
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = true;
                    window.Close();
                    break;
                }
            }
        }
    }
}