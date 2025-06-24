﻿namespace ShoppingWeb.DTOs.User
{
    public class UserProfileResponseDTO
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateTime? CreatedAt { get; set; }


    }
}
