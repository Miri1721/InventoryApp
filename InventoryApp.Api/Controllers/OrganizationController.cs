using InventoryApp.Api.DTOs;
using InventoryApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace InventoryApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public OrganizationController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var organizations = _mongoDbService.Organizations
                .Find(o => o.IsActive)
                .ToList();

            var response = organizations
                .OrderBy(o => o.Name)
                .Select(o => new OrganizationResponseDto
                {
                    OrganizationId = o.OrganizationId,
                    Name = o.Name,
                    Type = o.Type
                });

            return Ok(response);
        }
    }
}