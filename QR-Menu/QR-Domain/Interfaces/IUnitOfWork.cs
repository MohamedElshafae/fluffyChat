using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using QR_Domain.Entities;

namespace QR_Domain.Interfaces
{
    public interface IUnitOfWork
    {
        public IBaseRepository<User> UserRepository { get; set;} 
        public IBaseRepository<Tag> TagRepository { get; set;}
        public IBaseRepository<IdentityRole<Guid>> RoleRepository { get; set; }
        public Task SaveChangesAsync();

    }
}
