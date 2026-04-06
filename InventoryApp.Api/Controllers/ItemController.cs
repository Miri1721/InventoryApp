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

            var normalizedName = request.Name.Trim();

            var existingItem = _mongoDbService.Items
                .Find(i =>
                    i.OrganizationId == request.OrganizationId &&
                    i.CategoryId == request.CategoryId &&
                    !i.IsDeleted &&
                    i.Name.ToLower() == normalizedName.ToLower())
                .FirstOrDefault();

            if (existingItem != null)
            {
                return BadRequest("An item with this name already exists in this category.");
            }

            var item = new Item
            {
                Name = normalizedName,
                Description = request.Description,
                CategoryId = request.CategoryId,
                OrganizationId = request.OrganizationId,
                Unit = request.Unit,
                CurrentQuantity = request.CurrentQuantity,
                MinimumThreshold = request.MinimumThreshold,
                Supplier = request.Supplier,
                SupplierPhone = request.SupplierPhone,
                SupplierEmail = request.SupplierEmail
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
                Supplier = item.Supplier,
                SupplierPhone = item.SupplierPhone,
                SupplierEmail = item.SupplierEmail,
                IsBelowThreshold = item.CurrentQuantity < item.MinimumThreshold,
                IsActive = item.IsActive,
                IsDeleted = item.IsDeleted
            });
        }

        [HttpGet("organization/{organizationId}")]
        public IActionResult GetByOrganization(Guid organizationId)
        {
            var items = _mongoDbService.Items
                .Find(i => i.OrganizationId == organizationId && !i.IsDeleted)
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
                Supplier = i.Supplier,
                SupplierPhone = i.SupplierPhone,
                SupplierEmail = i.SupplierEmail,
                IsBelowThreshold = i.CurrentQuantity < i.MinimumThreshold,
                IsActive = i.IsActive,
                IsDeleted = i.IsDeleted
            });

            return Ok(response);
        }

        [HttpGet("shortage/{organizationId}")]
        public IActionResult GetShortageItems(Guid organizationId)
        {
            var items = _mongoDbService.Items
                .Find(i =>
                    i.OrganizationId == organizationId &&
                    !i.IsDeleted &&
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
                Supplier = i.Supplier,
                SupplierPhone = i.SupplierPhone,
                SupplierEmail = i.SupplierEmail,
                IsBelowThreshold = true,
                IsActive = i.IsActive,
                IsDeleted = i.IsDeleted
            });

            return Ok(response);
        }

        [HttpPut("{itemId}")]
        public IActionResult Update(Guid itemId, [FromBody] UpdateItemRequestDto request)
        {
            var item = _mongoDbService.Items
                      .Find(i => i.ItemId == itemId && !i.IsDeleted && i.IsActive)
                      .FirstOrDefault();

            var normalizedName = request.Name.Trim();

            var duplicateItem = _mongoDbService.Items
                .Find(i =>
                    i.ItemId != itemId &&
                    i.OrganizationId == item.OrganizationId &&
                    i.CategoryId == item.CategoryId &&
                    !i.IsDeleted &&
                    i.Name.ToLower() == normalizedName.ToLower())
                .FirstOrDefault();

            if (duplicateItem != null)
            {
                return BadRequest("An item with this name already exists in this category.");
            }

            if (item == null)
            {
                return NotFound("Item not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Item name is required.");
            }

            var update = Builders<Item>.Update
                 .Set(i => i.Name, normalizedName)
                 .Set(i => i.Description, request.Description)
                 .Set(i => i.Unit, request.Unit)
                 .Set(i => i.CurrentQuantity, request.CurrentQuantity)
                 .Set(i => i.MinimumThreshold, request.MinimumThreshold)
                 .Set(i => i.Supplier, request.Supplier)
                 .Set(i => i.SupplierPhone, request.SupplierPhone)
                 .Set(i => i.SupplierEmail, request.SupplierEmail);

            _mongoDbService.Items.UpdateOne(
                i => i.ItemId == itemId,
                update);

            item.Name = normalizedName;
            item.Description = request.Description;
            item.Unit = request.Unit;
            item.CurrentQuantity = request.CurrentQuantity;
            item.MinimumThreshold = request.MinimumThreshold;
            item.Supplier = request.Supplier;
            item.SupplierPhone = request.SupplierPhone;
            item.SupplierEmail = request.SupplierEmail;

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
                Supplier = item.Supplier,
                SupplierPhone = item.SupplierPhone,
                SupplierEmail = item.SupplierEmail,
                IsBelowThreshold = item.CurrentQuantity < item.MinimumThreshold,
                IsActive = item.IsActive,
                IsDeleted = item.IsDeleted
            });
        }

        //[HttpDelete("{itemId}")]
        //public IActionResult Deactivate(Guid itemId)
        //{
        //    var item = _mongoDbService.Items
        //        .Find(i => i.ItemId == itemId && i.IsActive)
        //        .FirstOrDefault();

        //    if (item == null)
        //    {
        //        return NotFound("Item not found.");
        //    }

        //    var update = Builders<Item>.Update
        //        .Set(i => i.IsActive, false);

        //    _mongoDbService.Items.UpdateOne(
        //        i => i.ItemId == itemId,
        //        update);

        //    return Ok("Item deactivated successfully.");
        //}

        [HttpPut("deactivate/{itemId}")]
        public IActionResult Deactivate(Guid itemId)
        {
            var item = _mongoDbService.Items
                .Find(i => i.ItemId == itemId && !i.IsDeleted && i.IsActive)
                .FirstOrDefault();

            if (item == null)
            {
                return NotFound("Active item not found.");
            }

            var update = Builders<Item>.Update
                .Set(i => i.IsActive, false);

            _mongoDbService.Items.UpdateOne(
                i => i.ItemId == itemId,
                update);

            return Ok("Item deactivated successfully.");
        }

        [HttpPut("reactivate/{itemId}")]
        public IActionResult Reactivate(Guid itemId)
        {
            var item = _mongoDbService.Items
                .Find(i => i.ItemId == itemId && !i.IsDeleted && !i.IsActive)
                .FirstOrDefault();

            if (item == null)
            {
                return NotFound("Deactivated item not found.");
            }

            var update = Builders<Item>.Update
                .Set(i => i.IsActive, true);

            _mongoDbService.Items.UpdateOne(
                i => i.ItemId == itemId,
                update);

            return Ok("Item reactivated successfully.");
        }

        [HttpDelete("{itemId}")]
        public IActionResult Delete(Guid itemId)
        {
            var item = _mongoDbService.Items
                .Find(i => i.ItemId == itemId && !i.IsDeleted)
                .FirstOrDefault();

            if (item == null)
            {
                return NotFound("Item not found.");
            }

            var update = Builders<Item>.Update
                .Set(i => i.IsDeleted, true);

            _mongoDbService.Items.UpdateOne(
                i => i.ItemId == itemId,
                update);

            return Ok("Item deleted successfully.");
        }
    }
}