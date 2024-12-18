using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using QR_Domain.Entities;
using QR_Domain.Enums;
using QR_Domain.Interfaces;

namespace QR.Application.Services
{
    public class RoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }
        public async Task<IdentityRole<Guid>> CreateRole(Roles roleType)
        {
            var role = await _unitOfWork.RoleRepository.FindAsync(r => r.Name == roleType.ToString());

            if (role is null)
            {
                role = new IdentityRole<Guid>() { Id = Guid.NewGuid(), Name = roleType.ToString() };
                await _unitOfWork.RoleRepository.AddAsync(role);
                await _unitOfWork.SaveChangesAsync();
            }

            return role;
        }
    }
}
