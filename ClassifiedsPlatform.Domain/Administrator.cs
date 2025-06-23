using System;

namespace ClassifiedsPlatform.Domain
{
    public class Administrator : User
    {
        // Метод для схвалення оголошення
        public void ApproveAd(Ad ad)
        {
            if (ad == null) return;
            ad.Status = AdStatus.Active;
        }

        // Метод для відхилення оголошення
        public void RejectAd(Ad ad)
        {
            if (ad == null) return;
            ad.Status = AdStatus.Rejected;
        }

        // Метод для блокування користувача
        public void BlockUser(User user)
        {
            if (user == null) return;
            user.Status = UserStatus.Blocked;
        }
    }
}