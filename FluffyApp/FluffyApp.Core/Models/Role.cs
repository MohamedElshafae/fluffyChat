using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FluffyApp.Core.Models
{
    public class Role: IdentityRole<Guid>
    {
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
