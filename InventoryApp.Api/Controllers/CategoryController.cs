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
                OrganizationId = category.OrganizationId,
                IsActive = category.IsActive,
                IsDeleted = category.IsDeleted
            });
        }

        [HttpGet("organization/{organizationId}")]
        public IActionResult GetByOrganization(Guid organizationId)
        {
            var categories = _mongoDbService.Categories
                .Find(c => c.OrganizationId == organizationId && !c.IsDeleted)
                .ToList();

            var response = categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                OrganizationId = c.OrganizationId,
                IsActive = c.IsActive,
                IsDeleted = c.IsDeleted
            });

            return Ok(response);
        }

        [HttpPut("{categoryId}")]
        public IActionResult Update(Guid categoryId, [FromBody] UpdateCategoryRequestDto request)
        {
            var category = _mongoDbService.Categories
                .Find(c => c.CategoryId == categoryId && !c.IsDeleted && c.IsActive)
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
                OrganizationId = category.OrganizationId,
                IsActive = category.IsActive,
                IsDeleted = category.IsDeleted
            });
        }

        [HttpPut("deactivate/{categoryId}")]
        public IActionResult Deactivate(Guid categoryId)
        {
            var category = _mongoDbService.Categories
                .Find(c => c.CategoryId == categoryId && !c.IsDeleted && c.IsActive)
                .FirstOrDefault();

            if (category == null)
            {
                return NotFound("Active category not found.");
            }

            var categoryUpdate = Builders<Category>.Update
                .Set(c => c.IsActive, false);

            _mongoDbService.Categories.UpdateOne(
                c => c.CategoryId == categoryId,
                categoryUpdate);

            var itemFilter = Builders<Item>.Filter.And(
                Builders<Item>.Filter.Eq(i => i.CategoryId, categoryId),
                Builders<Item>.Filter.Eq(i => i.IsDeleted, false)
            );

            var itemUpdate = Builders<Item>.Update
                .Set(i => i.IsActive, false);

            _mongoDbService.Items.UpdateMany(itemFilter, itemUpdate);

            return Ok("Category deactivated successfully.");
        }

        [HttpPut("reactivate/{categoryId}")]
        public IActionResult Reactivate(Guid categoryId)
        {
            var category = _mongoDbService.Categories
                .Find(c => c.CategoryId == categoryId && !c.IsDeleted && !c.IsActive)
                .FirstOrDefault();

            if (category == null)
            {
                return NotFound("Deactivated category not found.");
            }

            var update = Builders<Category>.Update
                .Set(c => c.IsActive, true);

            _mongoDbService.Categories.UpdateOne(
                c => c.CategoryId == categoryId,
                update);

            return Ok("Category reactivated successfully.");
        }

        [HttpDelete("{categoryId}")]
        public IActionResult Delete(Guid categoryId)
        {
            var category = _mongoDbService.Categories
                .Find(c => c.CategoryId == categoryId && !c.IsDeleted)
                .FirstOrDefault();

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            var categoryUpdate = Builders<Category>.Update
                .Set(c => c.IsDeleted, true);

            _mongoDbService.Categories.UpdateOne(
                c => c.CategoryId == categoryId,
                categoryUpdate);

            var itemFilter = Builders<Item>.Filter.And(
                Builders<Item>.Filter.Eq(i => i.CategoryId, categoryId),
                Builders<Item>.Filter.Eq(i => i.IsDeleted, false)
            );

            var itemUpdate = Builders<Item>.Update
                .Set(i => i.IsDeleted, true);

            _mongoDbService.Items.UpdateMany(itemFilter, itemUpdate);

            return Ok("Category deleted successfully.");
        }
    }
}