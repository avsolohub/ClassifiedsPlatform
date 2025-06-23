using System.Collections.Generic;

namespace ClassifiedsPlatform.Domain
{
    public interface IDataService
    {
        void SaveData(IEnumerable<User> users, IEnumerable<Ad> ads, IEnumerable<Category> categories, string filePath);
        (IEnumerable<User> users, IEnumerable<Ad> ads, IEnumerable<Category> categories) LoadData(string filePath);

       
    }
}