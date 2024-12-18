using HR.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QR.Application.DTOs;
using QR_Domain.Entities;
using QR_Domain.Enums;
using QR_Domain.Interfaces;


namespace QR.API.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(AuthService authService, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _unitOfWork = unitOfWork;
        }

        [Route("api/auth/login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _authService.Login(request);

            return StatusCode((int)result.StatusCode, result);
        }

        [Route("api/auth/register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _authService.Register(request);

            return StatusCode((int)result.StatusCode, result);
        }

        [Route("api/tags")]
        [HttpPost]
        public async Task<IActionResult> GetAllTags()
        {
            var existingTags = (await _unitOfWork.TagRepository.GetAllAsync()).ToList();

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
                }).ToList();

                await _unitOfWork.TagRepository.AddRangeAsync(newTags);
                await _unitOfWork.SaveChangesAsync();
            }

            var allTags = (await _unitOfWork.TagRepository.GetAllAsync()).ToList();
            return Ok(allTags);
        }

    }
}
