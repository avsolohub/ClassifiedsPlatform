﻿namespace ClassifiedsPlatform.Domain
{
    public abstract class User : BaseEntity
    {
        public string? Email { get; set; } 
        public string? Password { get; set; } 
        public UserStatus Status { get; set; }
    }
}