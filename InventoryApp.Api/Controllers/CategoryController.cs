using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;
using InventoryApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace InventoryApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public CategoryController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateCategoryRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Category name is required.");
            }

            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                OrganizationId = request.OrganizationId
            };

            _mongoDbService.Categories.InsertOne(category);

            return Ok(new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                OrganizationId = category.OrganizationId
            });
        }

        [HttpGet("organization/{organizationId}")]
        public IActionResult GetByOrganization(Guid organizationId)
        {
            var categories = _mongoDbService.Categories
                .Find(c => c.OrganizationId == organizationId && c.IsActive)
                .ToList();

            var response = categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                OrganizationId = c.OrganizationId
            });

            return Ok(response);
        }

        [HttpPut("{categoryId}")]
        public IActionResult Update(Guid categoryId, [FromBody] UpdateCategoryRequestDto request)
        {
            var category = _mongoDbService.Categories
                .Find(c => c.CategoryId == categoryId && c.IsActive)
                .FirstOrDefault();

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Category name is required.");
            }

            var update = Builders<Category>.Update
                .Set(c => c.Name, request.Name)
                .Set(c => c.Description, request.Description);

            _mongoDbService.Categories.UpdateOne(
                c => c.CategoryId == categoryId,
                update);

            category.Name = request.Name;
            category.Description = request.Description;

            return Ok(new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                OrganizationId = category.OrganizationId
            });
        }

        [HttpDelete("{categoryId}")]
        public IActionResult Deactivate(Guid categoryId)
        {
            var category = _mongoDbService.Categories
                .Find(c => c.CategoryId == categoryId && c.IsActive)
                .FirstOrDefault();

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            var update = Builders<Category>.Update
                .Set(c => c.IsActive, false);

            _mongoDbService.Categories.UpdateOne(
                c => c.CategoryId == categoryId,
                update);

            return Ok("Category deactivated successfully.");
        }
    }
}