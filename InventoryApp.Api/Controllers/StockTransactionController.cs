using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;
using InventoryApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace InventoryApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockTransactionController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public StockTransactionController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateStockTransactionRequestDto request)
        {
            if (request.ItemId == Guid.Empty)
                return BadRequest("ItemId is required.");

            if (request.OrganizationId == Guid.Empty)
                return BadRequest("OrganizationId is required.");

            if (string.IsNullOrWhiteSpace(request.TransactionType))
                return BadRequest("TransactionType is required.");

            if (request.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            var allowedTypes = new[] { "Restock", "Usage", "Waste", "Correction" };

            if (!allowedTypes.Contains(request.TransactionType))
                return BadRequest("Invalid transaction type.");

            var item = _mongoDbService.Items
                .Find(i => i.ItemId == request.ItemId &&
                           i.OrganizationId == request.OrganizationId &&
                           i.IsActive)
                .FirstOrDefault();

            if (item == null)
                return NotFound("Item not found.");

            double quantityBefore = item.CurrentQuantity;
            double quantityChange;

            switch (request.TransactionType)
            {
                case "Restock":
                    quantityChange = request.Quantity;
                    break;

                case "Usage":
                case "Waste":
                    quantityChange = -request.Quantity;
                    break;

                case "Correction":
                    // For now correction means "add/subtract by entered amount"
                    // If later you prefer "set final exact quantity", we can change this design
                    quantityChange = request.Quantity;
                    break;

                default:
                    return BadRequest("Invalid transaction type.");
            }

            double quantityAfter = quantityBefore + quantityChange;

            if (quantityAfter < 0)
                return BadRequest("Stock cannot become negative.");

            var transaction = new StockTransaction
            {
                ItemId = item.ItemId,
                OrganizationId = item.OrganizationId,
                TransactionType = request.TransactionType,
                QuantityChange = quantityChange,
                QuantityBefore = quantityBefore,
                QuantityAfter = quantityAfter,
                Note = request.Note?.Trim() ?? string.Empty,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByEmail = request.CreatedByEmail?.Trim() ?? string.Empty
            };

            _mongoDbService.StockTransactions.InsertOne(transaction);

            var update = Builders<Item>.Update
                .Set(i => i.CurrentQuantity, quantityAfter);

            _mongoDbService.Items.UpdateOne(i => i.ItemId == item.ItemId, update);

            return Ok(new StockTransactionResponseDto
            {
                StockTransactionId = transaction.StockTransactionId,
                ItemId = transaction.ItemId,
                OrganizationId = transaction.OrganizationId,
                TransactionType = transaction.TransactionType,
                QuantityChange = transaction.QuantityChange,
                QuantityBefore = transaction.QuantityBefore,
                QuantityAfter = transaction.QuantityAfter,
                Note = transaction.Note,
                CreatedAtUtc = transaction.CreatedAtUtc,
                CreatedByEmail = transaction.CreatedByEmail
            });
        }

        [HttpGet("item/{itemId}")]
        public IActionResult GetByItem(Guid itemId)
        {
            var transactions = _mongoDbService.StockTransactions
                .Find(t => t.ItemId == itemId)
                .SortByDescending(t => t.CreatedAtUtc)
                .ToList();

            var response = transactions.Select(t => new StockTransactionResponseDto
            {
                StockTransactionId = t.StockTransactionId,
                ItemId = t.ItemId,
                OrganizationId = t.OrganizationId,
                TransactionType = t.TransactionType,
                QuantityChange = t.QuantityChange,
                QuantityBefore = t.QuantityBefore,
                QuantityAfter = t.QuantityAfter,
                Note = t.Note,
                CreatedAtUtc = t.CreatedAtUtc,
                CreatedByEmail = t.CreatedByEmail
            });

            return Ok(response);
        }
    }
}