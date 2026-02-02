using System; 

namespace Application.Models
{
      public record User
    {
        public int Id { get; init; }
        public string? Username { get; init; }
        public required string FirstName { get; init; }
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; } = null;
        public bool IsActive { get; init; } 
        public int Age { get; init; }   
    }
}