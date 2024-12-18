using FluffyApp.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FluffyApp.EF.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.Data;
using System.Net;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FluffyApp.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public AuthController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [HttpGet("roles")]
        public async Task<IActionResult> Role()
        {
            List<Role> newRoles = [new Role { Id = Guid.NewGuid(), Name = Roles.Instructor.ToString() }, new Role { Id = Guid.NewGuid(), Name = Roles.User.ToString() }];

            if (!await _context.Roles.AnyAsync())
            {
                await _context.Roles.AddRangeAsync(newRoles);
                await _context.SaveChangesAsync();
            }

            return Ok(newRoles);
        }
        [HttpGet("tags")]
        public async Task<IActionResult> GetAllTags()
        {
            var existingTags = await _context.Tags.ToListAsync();

            var enumTags = Enum.GetNames(typeof(Tags));

            var missingTags = enumTags
                .Where(tagName => !existingTags.Any(t => t.Name == tagName))
                .ToList();

            if (missingTags.Any())
            {
                var newTags = missingTags.Select(tagName => new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = tagName
                });

                await _context.Tags.AddRangeAsync(newTags);
                await _context.SaveChangesAsync();
            }

            var allTags = await _context.Tags.ToListAsync();
            return Ok(allTags);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest("Email already in use.");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role.ToString());

            if (role == null)
                return Conflict("no Role Found");

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Bio = request.Bio,
                Certificates = request.Certificates,
                Education = request.Education,
                Experience = request.Experience,
                Facebook = request.Facebook,
                LinkedIn = request.LinkedIn,
                RoleId = role.Id,
                Twitter = request.Twitter,
                PhoneNumber = request.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            if (request.TagIds != null && request.TagIds.Any())
            {
                var tags = await _context.Tags.Where(t => request.TagIds.Contains(t.Id)).ToListAsync();

                user.Tags = tags;
                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Registration successful!" });
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("TokenId", Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("qwfBXYx9Ukr8xUnH45mWlH3k5YmQZ1O6d0nFQf4B2XGVUJDrBLOi4Zax6e+fVOMT4ZrtZzGwlOoRZP/kd+EYsQ==,"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return StatusCode(401, "Invalid User");
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return StatusCode(401, "Invalid Password");
            return Ok(new loginRespone(user.FirstName, user.LastName,await GenerateJwtToken(user)));
        }
    }
 }

public class loginRespone
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Token { get; set; }
    public loginRespone(string firstName, string lastName, string token)
    {
        FirstName = firstName;
        LastName = LastName;
        Token = token;
    }

}
public class RegisterDto
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

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    public enum Roles
    {
        User,
        Instructor
    }

    public enum Tags
    {
        Math,
        Science,
        History,
        English,
        ComputerScience,
        Backend,
        FrontEnd,
    }

