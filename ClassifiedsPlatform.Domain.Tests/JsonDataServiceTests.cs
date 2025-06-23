using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClassifiedsPlatform.Domain.Tests
{
    [TestClass]
    public class JsonDataServiceTests
    {
        [TestMethod]
        public void SaveData_And_LoadData_ShouldReturnSameData()
        {
            // Arrange
            var service = new JsonDataService();
            var testFilePath = "test_data.json";

            var users = new List<User> { new RegisteredUser { Id = 1, Email = "test@test.com" } };
            var ads = new List<Ad> { new Ad { Id = 1, Title = "Test Ad" } };
            var categories = new List<Category> { new Category { Id = 1, Name = "Test Category" } };

            // Act & Assert
            try
            {
                // Act 1: Save data
                service.SaveData(users, ads, categories, testFilePath);

                // Act 2: Load data
                var (loadedUsers, loadedAds, loadedCategories) = service.LoadData(testFilePath);

                // Assert: Check if loaded data matches saved data
                Assert.AreEqual(users.Count, loadedUsers.Count());
                Assert.AreEqual(ads.Count, loadedAds.Count());
                Assert.AreEqual(categories.Count, loadedCategories.Count());
                Assert.AreEqual("test@test.com", loadedUsers.First().Email);
            }
            finally
            {
                // Cleanup: Delete the test file after the test runs
                if (File.Exists(testFilePath))
                {
                    File.Delete(testFilePath);
                }
            }
        }
    }
}