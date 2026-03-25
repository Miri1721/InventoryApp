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


        [HttpPut("{itemId}")]
        public IActionResult Update(Guid itemId, [FromBody] UpdateItemRequestDto request)
        {
            var item = _mongoDbService.Items
                .Find(i => i.ItemId == itemId && i.IsActive)
                .FirstOrDefault();

            if (item == null)
            {
                return NotFound("Item not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Item name is required.");
            }

            var update = Builders<Item>.Update
                .Set(i => i.Name, request.Name)
                .Set(i => i.Description, request.Description)
                .Set(i => i.Unit, request.Unit)
                .Set(i => i.CurrentQuantity, request.CurrentQuantity)
                .Set(i => i.MinimumThreshold, request.MinimumThreshold);

            _mongoDbService.Items.UpdateOne(
                i => i.ItemId == itemId,
                update);

            item.Name = request.Name;
            item.Description = request.Description;
            item.Unit = request.Unit;
            item.CurrentQuantity = request.CurrentQuantity;
            item.MinimumThreshold = request.MinimumThreshold;

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

        [HttpDelete("{itemId}")]
        public IActionResult Deactivate(Guid itemId)
        {
            var item = _mongoDbService.Items
                .Find(i => i.ItemId == itemId && i.IsActive)
                .FirstOrDefault();

            if (item == null)
            {
                return NotFound("Item not found.");
            }

            var update = Builders<Item>.Update
                .Set(i => i.IsActive, false);

            _mongoDbService.Items.UpdateOne(
                i => i.ItemId == itemId,
                update);

            return Ok("Item deactivated successfully.");
        }
    }
}