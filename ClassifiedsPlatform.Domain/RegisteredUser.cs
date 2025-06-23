using System;

namespace ClassifiedsPlatform.Domain
{
    public class RegisteredUser : User
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }

        // Метод для створення нового оголошення
        public Ad CreateAd(string title, string description, decimal price, Category category)
        {
            // Створюємо новий об'єкт оголошення і заповнюємо його даними
            var newAd = new Ad
            {
                Title = title,
                Description = description,
                Price = price,
                CategoryId = category.Id,
                AuthorId = this.Id, // Id самого користувача, який створює оголошення
                CreationDate = DateTime.Now,
                Status = AdStatus.PendingModeration // За замовчуванням відправляємо на модерацію
            };

            return newAd;
        }

        // Метод для редагування існуючого оголошення
        public void EditAd(Ad ad, string newTitle, string newDescription, decimal newPrice)
        {
            if (ad == null) return;

            // Оновлюємо властивості переданого оголошення
            ad.Title = newTitle;
            ad.Description = newDescription;
            ad.Price = newPrice;
        }

        // Метод для видалення оголошення
        public void DeleteAd(Ad ad)
        {
            
        }
    }
}