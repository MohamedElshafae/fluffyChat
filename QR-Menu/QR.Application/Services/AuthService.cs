using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using QR_Domain.Entities;
using QR_Domain.Interfaces;
using QR.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using QR_Domain.Enums;
using QR.Application.Services;

namespace HR.Application.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleService _roleService;
        public AuthService(IConfiguration configuration, UserManager<User> userManager, RoleService roleService , IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleService = roleService;
            _unitOfWork = unitOfWork;
        }
        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.Name),
                new Claim("TokenId", Guid.NewGuid().ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration["JWT:SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task<Result<string>> Register(RegisterRequest registerRequest)
        {
            // Check if the email already exists
            var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
  
            if (existingUser != null)
                return new Result<string>("Email already in use.", HttpStatusCode.BadRequest);


            // Create the user object
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Bio = registerRequest.Bio,
                Certificates = registerRequest.Certificates,
                Education = registerRequest.Education,
                Experience = registerRequest.Experience,
                Facebook = registerRequest.Facebook,
                LinkedIn = registerRequest.LinkedIn,
                Twitter = registerRequest.Twitter,
                PhoneNumber = registerRequest.PhoneNumber,
                Role = await _roleService.CreateRole(registerRequest.Role)
            };

            // Create the user asynchronously
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                // Return the error messages from the result
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return new Result<string>(errorMessage, HttpStatusCode.BadRequest);
            }

            // If there are tags to associate with the user
            if (registerRequest.TagIds != null && registerRequest.TagIds.Any())
            {
                var tags = await _unitOfWork.TagRepository.GetAllByAsync(t => registerRequest.TagIds.Contains(t.Id));

                user.Tags = tags.ToList();
            }

            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Return success message
            return new Result<string>("Registration successful!", HttpStatusCode.OK);
        }


        public async Task<Result<string>> Login(LoginRequest loginDTO)
        {
            var user = await _unitOfWork.UserRepository.FindAsync(u => u.Email == loginDTO.Email, ["Role"]);

            if (user == null)
                return new Result<string>("Invalid User", HttpStatusCode.Unauthorized);

            if (!await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                return new Result<string>("Invalid Password", HttpStatusCode.Unauthorized);

            return new Result<string>(await GenerateJwtToken(user), "Success", HttpStatusCode.OK);
        }



    }
}


