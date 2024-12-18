using System.ComponentModel.DataAnnotations;
using QR_Domain.Enums;

namespace QR.Application.DTOs
{
    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Bio { get; set; }
        public string PhoneNumber { get; set; }
        public List<Guid> TagIds { get; set; } = new List<Guid>();
        public Roles Role { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public string? Certificates { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? LinkedIn { get; set; }
        public string? Youtube { get; set; }
    }
}