using ClassifiedsPlatform.Domain;

namespace ClassifiedsPlatform.WPF.ViewModels
{
    public class AdViewModel : BaseViewModel
    {
        public Ad Ad { get; }

        public int Id => Ad.Id;
        public string? Title => Ad.Title;
        public string? Description => Ad.Description;
        public decimal Price => Ad.Price;

        public string CategoryName { get; } 
        public bool IsOwner { get; }

        
        public AdViewModel(Ad ad, int? currentUserId, string categoryName)
        {
            Ad = ad;
            IsOwner = currentUserId.HasValue && ad.AuthorId == currentUserId.Value;
            CategoryName = categoryName; 
        }
    }
}