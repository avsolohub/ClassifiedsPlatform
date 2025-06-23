using Microsoft.VisualStudio.TestTools.UnitTesting;
using System; // Потрібно для Exception

namespace ClassifiedsPlatform.Domain.Tests
{
    [TestClass]
    public class RegisteredUserTests
    {
        [TestMethod]
        public void CreateAd_WithValidData_ShouldReturnAdWithPendingStatus()
        {
            // Arrange
            var user = new RegisteredUser();
            var category = new Category { Id = 1, Name = "Electronics" };

            // Act
            var newAd = user.CreateAd("Test Ad", "Description", 100m, category);

            // Assert
            Assert.IsNotNull(newAd);
            Assert.AreEqual(AdStatus.PendingModeration, newAd.Status);
        }

        [TestMethod]
        public void EditAd_WhenUserIsOwner_ShouldUpdateAdProperties()
        {
            // Arrange
            var user = new RegisteredUser();
            var ad = new Ad { Title = "Old Title", Price = 50m };
            var newTitle = "New Title";
            var newPrice = 150m;

            // Act
            user.EditAd(ad, newTitle, "New Description", newPrice);

            // Assert
            Assert.AreEqual(newTitle, ad.Title);
            Assert.AreEqual(newPrice, ad.Price);
        }

        // Цей тест перевіряє, що метод не кидає виняток, якщо все добре
        [TestMethod]
        public void DeleteAd_WhenUserIsOwner_ShouldSucceed()
        {
            // Arrange
            var user = new RegisteredUser();
            var ad = new Ad();

            // Act
            // Якщо метод DeleteAd кидає виняток, тест провалиться.
            // Якщо ні - пройде.
            user.DeleteAd(ad);

            // Assert
            Assert.IsTrue(true);
        }
    }
}