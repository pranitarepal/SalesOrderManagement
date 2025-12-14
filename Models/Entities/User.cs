using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrderManagement.Models.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? ContactNo { get; set; }

        public string? Address { get; set; }

        public string? ZipCode { get; set; }

        public string? UserType { get; set; }
    }
}
