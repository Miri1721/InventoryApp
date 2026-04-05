using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;
using InventoryApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

using InventoryApp.Api.DTOs;

namespace InventoryApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public UserController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpGet("organization/{organizationId}")]
        public IActionResult GetActiveByOrganization(Guid organizationId)
        {
            var users = _mongoDbService.Users
                .Find(u => u.OrganizationId == organizationId &&
                           u.IsActive &&
                           u.Role != "Manager")
                .ToList();

            var response = users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                OrganizationId = u.OrganizationId,
                IsActive = u.IsActive
            });

            return Ok(response);
        }

        [HttpGet("deleted/organization/{organizationId}")]
        public IActionResult GetDeletedByOrganization(Guid organizationId)
        {
            var users = _mongoDbService.Users
                .Find(u => u.OrganizationId == organizationId &&
                           !u.IsActive &&
                           u.Role != "Manager")
                .ToList();

            var response = users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                OrganizationId = u.OrganizationId,
                IsActive = u.IsActive
            });

            return Ok(response);
        }

        [HttpPut("deactivate/{userId}")]
        public IActionResult Deactivate(Guid userId)
        {
            var user = _mongoDbService.Users
                .Find(u => u.UserId == userId && u.IsActive)
                .FirstOrDefault();

            if (user == null)
                return NotFound("Active user not found.");

            var update = Builders<User>.Update
                .Set(u => u.IsActive, false);

            _mongoDbService.Users.UpdateOne(
                u => u.UserId == userId,
                update);

            return Ok("User deactivated successfully.");
        }

        [HttpPut("reactivate/{userId}")]
        public IActionResult Reactivate(Guid userId)
        {
            var user = _mongoDbService.Users
                .Find(u => u.UserId == userId && !u.IsActive)
                .FirstOrDefault();

            if (user == null)
                return NotFound("Deleted user not found.");

            var update = Builders<User>.Update
                .Set(u => u.IsActive, true);

            _mongoDbService.Users.UpdateOne(
                u => u.UserId == userId,
                update);

            return Ok("User reactivated successfully.");
        }
    }
}