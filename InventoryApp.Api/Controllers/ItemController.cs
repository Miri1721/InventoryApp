using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;
using InventoryApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace InventoryApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public ItemController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateItemRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Item name is required.");
            }

            var item = new Item
            {
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryId,
                OrganizationId = request.OrganizationId,
                Unit = request.Unit,
                CurrentQuantity = request.CurrentQuantity,
                MinimumThreshold = request.MinimumThreshold
            };

            _mongoDbService.Items.InsertOne(item);

            return Ok(new ItemResponseDto
            {
                ItemId = item.ItemId,
                Name = item.Name,
                Description = item.Description,
                CategoryId = item.CategoryId,
                OrganizationId = item.OrganizationId,
                Unit = item.Unit,
                CurrentQuantity = item.CurrentQuantity,
                MinimumThreshold = item.MinimumThreshold,
                IsBelowThreshold = item.CurrentQuantity < item.MinimumThreshold
            });
        }

        [HttpGet("organization/{organizationId}")]
        public IActionResult GetByOrganization(Guid organizationId)
        {
            var items = _mongoDbService.Items
                .Find(i => i.OrganizationId == organizationId && i.IsActive)
                .ToList();

            var response = items.Select(i => new ItemResponseDto
            {
                ItemId = i.ItemId,
                Name = i.Name,
                Description = i.Description,
                CategoryId = i.CategoryId,
                OrganizationId = i.OrganizationId,
                Unit = i.Unit,
                CurrentQuantity = i.CurrentQuantity,
                MinimumThreshold = i.MinimumThreshold,
                IsBelowThreshold = i.CurrentQuantity < i.MinimumThreshold
            });

            return Ok(response);
        }

        [HttpGet("shortage/{organizationId}")]
        public IActionResult GetShortageItems(Guid organizationId)
        {
            var items = _mongoDbService.Items
                .Find(i =>
                    i.OrganizationId == organizationId &&
                    i.IsActive &&
                    i.CurrentQuantity < i.MinimumThreshold)
                .ToList();

            var response = items.Select(i => new ItemResponseDto
            {
                ItemId = i.ItemId,
                Name = i.Name,
                Description = i.Description,
                CategoryId = i.CategoryId,
                OrganizationId = i.OrganizationId,
                Unit = i.Unit,
                CurrentQuantity = i.CurrentQuantity,
                MinimumThreshold = i.MinimumThreshold,
                IsBelowThreshold = true
            });

            return Ok(response);
        }
    }
}