using Newtonsoft.Json; // Підключаємо встановлену бібліотеку
using System;
using System.Collections.Generic;
using System.IO; // Підключаємо бібліотеку для роботи з файлами

namespace ClassifiedsPlatform.Domain
{
    public class JsonDataService : IDataService
    {
        // Налаштування для серіалізації, які допомагають правильно обробляти спадкування
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto, // Додає в JSON інформацію про конкретний тип (Administrator чи RegisteredUser)
            Formatting = Formatting.Indented // Робить JSON-файл читабельним для людини
        };

        public (IEnumerable<User> users, IEnumerable<Ad> ads, IEnumerable<Category> categories) LoadData(string filePath)
        {
            // Якщо файл не існує, повертаємо порожні списки
            if (!File.Exists(filePath))
            {
                return (new List<User>(), new List<Ad>(), new List<Category>());
            }

            // Читаємо весь текст з файлу
            var json = File.ReadAllText(filePath);

            // Створюємо анонімний тип для зручної десеріалізації
            var data = new
            {
                Users = new List<User>(),
                Ads = new List<Ad>(),
                Categories = new List<Category>()
            };

            // Десеріалізуємо (перетворюємо текст з JSON на об'єкти)
            var deserializedData = JsonConvert.DeserializeAnonymousType(json, data, _settings);

            return (deserializedData.Users, deserializedData.Ads, deserializedData.Categories);
        }

        public void SaveData(IEnumerable<User> users, IEnumerable<Ad> ads, IEnumerable<Category> categories, string filePath)
        {
            // Створюємо анонімний об'єкт, який будемо зберігати
            var dataToSave = new
            {
                Users = users,
                Ads = ads,
                Categories = categories
            };

            // Серіалізуємо (перетворюємо наші об'єкти на текст у форматі JSON)
            var json = JsonConvert.SerializeObject(dataToSave, _settings);

            // Записуємо отриманий текст у файл
            File.WriteAllText(filePath, json);
        }
    }
}