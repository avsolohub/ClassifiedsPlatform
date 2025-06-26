using ClassifiedsPlatform.Domain;
using ClassifiedsPlatform.WPF.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ClassifiedsPlatform.WPF.ViewModels
{
    public class CreateAdViewModel : BaseViewModel
    {
        // Властивості для полів вводу
        private string? _title;
        public string? Title { get => _title; set { _title = value; OnPropertyChanged(); } }

        private string? _description;
        public string? Description { get => _description; set { _description = value; OnPropertyChanged(); } }

        private decimal _price;
        public decimal Price { get => _price; set { _price = value; OnPropertyChanged(); } }

        private Category? _selectedCategory;
        public Category? SelectedCategory { get => _selectedCategory; set { _selectedCategory = value; OnPropertyChanged(); } }

        public ObservableCollection<Category> AvailableCategories { get; set; }
        public Ad? AdToProcess { get; private set; } 

        public ICommand SaveCommand { get; }

        
        public CreateAdViewModel(IEnumerable<Category> categories, Ad? adToEdit = null)
        {
            AvailableCategories = new ObservableCollection<Category>(categories);
            AdToProcess = adToEdit; 

            if (adToEdit != null)
            {
                
                Title = adToEdit.Title;
                Description = adToEdit.Description;
                Price = adToEdit.Price;
                SelectedCategory = AvailableCategories.FirstOrDefault(c => c.Id == adToEdit.CategoryId);
            }

            SaveCommand = new RelayCommand(SaveChanges);
        }

        private void SaveChanges(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(Title) || SelectedCategory == null)
            {
                MessageBox.Show("Назва та категорія є обов'язковими.", "Помилка валідації");
                return;
            }

            
            if (AdToProcess == null)
            {
                AdToProcess = new Ad();
            }

            
            AdToProcess.Title = this.Title;
            AdToProcess.Description = this.Description;
            AdToProcess.Price = this.Price;
            AdToProcess.CategoryId = this.SelectedCategory.Id;
            AdToProcess.Status = AdStatus.PendingModeration;

            
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