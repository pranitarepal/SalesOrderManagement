using System;
using System.Text.Json.Serialization;

namespace SalesOrderManagement.Models.DTOs
{
    public class UserDetailsDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("contactNo")]
        public string? ContactNo { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("zipCode")]
        public string? ZipCode { get; set; }

        [JsonPropertyName("userType")]
        public string? UserType { get; set; }
    }
}
