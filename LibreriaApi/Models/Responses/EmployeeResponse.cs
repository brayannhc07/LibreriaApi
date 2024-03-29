﻿namespace LibreriaApi.Models.Responses
{
    public class EmployeeResponse
    {
        public EmployeeResponse(int id, string name, string role, string? address,
            string phoneNumber, string email, DateTime? birthday, string imageUrl)
        {
            Id = id;
            Name = name;
            Role = role;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
            Birthday = birthday;
            ImageUrl = imageUrl;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Role { get; private set; }
        public string? Address { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public DateTime? Birthday { get; private set; }
        public string ImageUrl { get; private set; }
    }
}
