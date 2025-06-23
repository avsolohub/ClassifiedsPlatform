using ClassifiedsPlatform.Domain; // Підключаємо простір імен з нашими класами
using Microsoft.VisualStudio.TestTools.UnitTesting; // Стандартна бібліотека для тестів

namespace ClassifiedsPlatform.Domain.Tests
{
    [TestClass] // Атрибут, який позначає, що це клас з тестами
    public class AdministratorTests
    {
        [TestMethod] // Атрибут, який позначає, що це конкретний тест-метод
        public void ApproveAd_WhenAdIsPending_ShouldChangeStatusToActive()
        {
            // Цей тест слідує класичній структурі "Arrange-Act-Assert"

            // 1. Arrange (Підготовка даних)
            // Створюємо об'єкти, необхідні для тесту.
            var administrator = new Administrator();
            var ad = new Ad { Status = AdStatus.PendingModeration }; // Створюємо оголошення з початковим статусом

            // 2. Act (Виконання дії)
            // Викликаємо метод, який ми тестуємо.
      
            administrator.ApproveAd(ad);

            // 3. Assert (Перевірка результату)
            // Перевіряємо, чи отриманий результат відповідає очікуваному.
            // До цього рядка виконання зараз не дійде.
            Assert.AreEqual(AdStatus.Active, ad.Status);
        }
        [TestMethod]
        public void BlockUser_WhenUserIsActive_ShouldChangeStatusToBlocked()
        {
            // Arrange
            var administrator = new Administrator();
            var userToBlock = new RegisteredUser { Status = UserStatus.Active };

            // Act
            administrator.BlockUser(userToBlock);

            // Assert
            Assert.AreEqual(UserStatus.Blocked, userToBlock.Status);
        }
        [TestMethod]
        public void RejectAd_WhenAdIsPending_ShouldChangeStatusToRejected()
        {
            // 1. Arrange (Підготовка)
            var administrator = new Administrator();
            var ad = new Ad { Status = AdStatus.PendingModeration };

            // 2. Act (Дія)
            // Цей виклик провалить тест, бо метод ще не реалізований
            administrator.RejectAd(ad);

            // 3. Assert (Перевірка)
            Assert.AreEqual(AdStatus.Rejected, ad.Status);
        }
    }
}