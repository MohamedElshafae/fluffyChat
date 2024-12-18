using Microsoft.AspNetCore.Identity;
using QR_Domain.Entities;
using QR_Domain.Interfaces;

namespace QR.DataLayer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QRContext _context;
        public IBaseRepository<User> UserRepository { get ; set ; }
        public IBaseRepository<Tag> TagRepository { get ; set ; }
        public IBaseRepository<IdentityRole<Guid>> RoleRepository { get ; set ; }

        public UnitOfWork(
            QRContext context,
            IBaseRepository<User> userRepository,
            IBaseRepository<Tag> tagRepository,
            IBaseRepository<IdentityRole<Guid>> roleRepository)
        {
            _context = context;
            UserRepository = userRepository;
            TagRepository = tagRepository;
            RoleRepository = roleRepository;
        }

        public Task SaveChangesAsync() =>
            _context.SaveChangesAsync();
    }
}
