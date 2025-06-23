using System;

namespace ClassifiedsPlatform.Domain
{
    public class Ad : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public AdStatus Status { get; set; }
        public DateTime CreationDate { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
    }
}