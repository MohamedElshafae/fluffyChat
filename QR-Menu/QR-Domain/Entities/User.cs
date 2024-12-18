using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR_Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public string? Certificates { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? LinkedIn { get; set; }
        public string? Youtube { get; set; }
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public IdentityRole<Guid> Role { get; set; }
    }
}
