using ClosedXML.Excel;
using InventoryApp.Api.Models;
using InventoryApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace InventoryApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public ReportController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpGet("excel/{organizationId}")]
        public IActionResult ExportExcel(Guid organizationId)
        {
            try
            {
                var organization = _mongoDbService.Organizations
                    .Find(o => o.OrganizationId == organizationId && o.IsActive)
                    .FirstOrDefault();

                if (organization == null)
                    return NotFound("Organization not found.");

                var categories = _mongoDbService.Categories
                    .Find(c => c.OrganizationId == organizationId && c.IsActive)
                    .ToList();

                var items = _mongoDbService.Items
                    .Find(i => i.OrganizationId == organizationId && i.IsActive)
                    .ToList();

                var transactions = _mongoDbService.StockTransactions
                    .Find(t => t.OrganizationId == organizationId)
                    .ToList();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Inventory Report");

                int row = 1;

                // Title
                worksheet.Cell(row, 1).Value = "Inventory Report";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 1).Style.Font.FontSize = 18;
                row += 2;

                // Organization summary
                worksheet.Cell(row, 1).Value = "Organization";
                worksheet.Cell(row, 2).Value = organization.Name;
                row++;

                worksheet.Cell(row, 1).Value = "Type";
                worksheet.Cell(row, 2).Value = organization.Type;
                row++;

                worksheet.Cell(row, 1).Value = "Generated At";
                worksheet.Cell(row, 2).Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                row++;

                worksheet.Cell(row, 1).Value = "Categories Count";
                worksheet.Cell(row, 2).Value = categories.Count;
                row++;

                worksheet.Cell(row, 1).Value = "Active Items Count";
                worksheet.Cell(row, 2).Value = items.Count;
                row++;

                worksheet.Cell(row, 1).Value = "Items To Reorder";
                worksheet.Cell(row, 2).Value = items.Count(i => i.CurrentQuantity < i.MinimumThreshold);
                row += 2;

                // Style summary labels
                for (int r = 3; r <= row - 2; r++)
                {
                    worksheet.Cell(r, 1).Style.Font.Bold = true;
                }

                // Go category by category
                foreach (var category in categories.OrderBy(c => c.Name))
                {
                    worksheet.Cell(row, 1).Value = $"Category: {category.Name}";
                    worksheet.Range(row, 1, row, 7).Merge();
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    row++;

                    worksheet.Cell(row, 1).Value = "Item Name";
                    worksheet.Cell(row, 2).Value = "Description";
                    worksheet.Cell(row, 3).Value = "Unit";
                    worksheet.Cell(row, 4).Value = "Current Quantity";
                    worksheet.Cell(row, 5).Value = "Minimum Threshold";
                    worksheet.Cell(row, 6).Value = "Status";
                    worksheet.Cell(row, 7).Value = "History Exists";

                    worksheet.Range(row, 1, row, 7).Style.Font.Bold = true;
                    worksheet.Range(row, 1, row, 7).Style.Fill.BackgroundColor = XLColor.LightGray;
                    row++;

                    var categoryItems = items
                        .Where(i => i.CategoryId == category.CategoryId)
                        .OrderBy(i => i.Name)
                        .ToList();

                    if (categoryItems.Count == 0)
                    {
                        worksheet.Cell(row, 1).Value = "No active items in this category.";
                        row += 2;
                        continue;
                    }

                    foreach (var item in categoryItems)
                    {
                        var itemTransactions = transactions
                            .Where(t => t.ItemId == item.ItemId)
                            .OrderByDescending(t => t.CreatedAtUtc)
                            .ToList();

                        worksheet.Cell(row, 1).Value = item.Name;
                        worksheet.Cell(row, 2).Value = item.Description;
                        worksheet.Cell(row, 3).Value = item.Unit;
                        worksheet.Cell(row, 4).Value = item.CurrentQuantity;
                        worksheet.Cell(row, 5).Value = item.MinimumThreshold;
                        worksheet.Cell(row, 6).Value = item.CurrentQuantity < item.MinimumThreshold ? "Below Threshold" : "OK";
                        worksheet.Cell(row, 7).Value = itemTransactions.Any() ? "Yes" : "No";

                        if (item.CurrentQuantity < item.MinimumThreshold)
                        {
                            worksheet.Range(row, 1, row, 7).Style.Fill.BackgroundColor = XLColor.LightPink;
                        }

                        row++;

                        // History section under the item
                        if (itemTransactions.Any())
                        {
                            worksheet.Cell(row, 2).Value = "History";
                            worksheet.Cell(row, 2).Style.Font.Bold = true;
                            row++;

                            worksheet.Cell(row, 2).Value = "Date";
                            worksheet.Cell(row, 3).Value = "Type";
                            worksheet.Cell(row, 4).Value = "Change";
                            worksheet.Cell(row, 5).Value = "Before";
                            worksheet.Cell(row, 6).Value = "After";
                            worksheet.Cell(row, 7).Value = "Note";
                            worksheet.Cell(row, 8).Value = "By";

                            worksheet.Range(row, 2, row, 8).Style.Font.Bold = true;
                            worksheet.Range(row, 2, row, 8).Style.Fill.BackgroundColor = XLColor.LightYellow;
                            row++;

                            foreach (var transaction in itemTransactions)
                            {
                                worksheet.Cell(row, 2).Value = transaction.CreatedAtUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                                worksheet.Cell(row, 3).Value = transaction.TransactionType;
                                worksheet.Cell(row, 4).Value = transaction.QuantityChange;
                                worksheet.Cell(row, 5).Value = transaction.QuantityBefore;
                                worksheet.Cell(row, 6).Value = transaction.QuantityAfter;
                                worksheet.Cell(row, 7).Value = transaction.Note;
                                worksheet.Cell(row, 8).Value = transaction.CreatedByEmail;
                                row++;
                            }
                        }

                        row++;
                    }

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"InventoryReport_{organization.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to generate report: {ex.Message}");
            }
        }
    }
}