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
                     .Find(i => i.OrganizationId == organizationId && !i.IsDeleted)
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
                worksheet.Cell(row, 2).Value = items.Count(i => i.IsActive);
                row++;

                worksheet.Cell(row, 1).Value = "Items To Reorder";
                worksheet.Cell(row, 2).Value = items.Count(i => i.IsActive && i.CurrentQuantity < i.MinimumThreshold);
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
                    worksheet.Range(row, 1, row, 8).Merge();
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    row++;

                    worksheet.Cell(row, 1).Value = "Item Name";
                    worksheet.Cell(row, 2).Value = "Description";
                    worksheet.Cell(row, 3).Value = "Unit";
                    worksheet.Cell(row, 4).Value = "Supplier";
                    worksheet.Cell(row, 5).Value = "Current Quantity";
                    worksheet.Cell(row, 6).Value = "Minimum Threshold";
                    worksheet.Cell(row, 7).Value = "Status";
                    worksheet.Cell(row, 8).Value = "History Exists";

                    worksheet.Range(row, 1, row, 8).Style.Font.Bold = true;
                    worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.LightGray;
                    row++;

                    var categoryItems = items
                        .Where(i => i.CategoryId == category.CategoryId)
                        .OrderBy(i => i.Name)
                        .ToList();

                    if (categoryItems.Count == 0)
                    {
                        worksheet.Cell(row, 1).Value = "No items in this category.";
                        row += 2;
                        continue;
                    }

                    foreach (var item in categoryItems)
                    {
                        if (!item.IsActive)
                        {
                            worksheet.Cell(row, 1).Value = item.Name;
                            worksheet.Cell(row, 2).Value = "Deactivated";
                            worksheet.Cell(row, 3).Value = "Deactivated";
                            worksheet.Cell(row, 4).Value = "Deactivated";
                            worksheet.Cell(row, 5).Value = "Deactivated";
                            worksheet.Cell(row, 6).Value = "Deactivated";
                            worksheet.Cell(row, 7).Value = "Deactivated";
                            worksheet.Cell(row, 8).Value = "Deactivated";

                            worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.LightGray;

                            row++;
                            row++;
                            continue;
                        }

                        var itemTransactions = transactions
                            .Where(t => t.ItemId == item.ItemId)
                            .OrderByDescending(t => t.CreatedAtUtc)
                            .ToList();

                        worksheet.Cell(row, 1).Value = item.Name;
                        worksheet.Cell(row, 2).Value = item.Description;
                        worksheet.Cell(row, 3).Value = item.Unit;
                        worksheet.Cell(row, 4).Value = item.Supplier;
                        worksheet.Cell(row, 5).Value = item.CurrentQuantity;
                        worksheet.Cell(row, 6).Value = item.MinimumThreshold;
                        worksheet.Cell(row, 7).Value = item.CurrentQuantity < item.MinimumThreshold ? "Below Threshold" : "OK";
                        worksheet.Cell(row, 8).Value = itemTransactions.Any() ? "Yes" : "No";

                        if (item.CurrentQuantity < item.MinimumThreshold)
                        {
                            worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.LightPink;
                        }

                        row++;

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

        [HttpGet("shortage-excel/{organizationId}")]
        public IActionResult ExportShortageExcel(Guid organizationId)
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

                var shortageItems = _mongoDbService.Items
                    .Find(i => i.OrganizationId == organizationId
                            && i.IsActive
                            && !i.IsDeleted
                            && i.CurrentQuantity < i.MinimumThreshold)
                    .ToList();

                var categoryMap = categories.ToDictionary(c => c.CategoryId, c => c.Name);

                using var workbook = new XLWorkbook();

                // Sheet 1: detailed shortage items
                var worksheet = workbook.Worksheets.Add("Items To Reorder");

                int row = 1;

                worksheet.Cell(row, 1).Value = "Items To Reorder Report";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 1).Style.Font.FontSize = 18;
                row += 2;

                worksheet.Cell(row, 1).Value = "Organization";
                worksheet.Cell(row, 2).Value = organization.Name;
                row++;

                worksheet.Cell(row, 1).Value = "Type";
                worksheet.Cell(row, 2).Value = organization.Type;
                row++;

                worksheet.Cell(row, 1).Value = "Generated At";
                worksheet.Cell(row, 2).Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                row++;

                worksheet.Cell(row, 1).Value = "Items To Reorder Count";
                worksheet.Cell(row, 2).Value = shortageItems.Count;
                row += 2;

                for (int r = 3; r <= row - 2; r++)
                {
                    worksheet.Cell(r, 1).Style.Font.Bold = true;
                }
                worksheet.Cell(row, 1).Value = "Category";
                worksheet.Cell(row, 2).Value = "Item Name";
                worksheet.Cell(row, 3).Value = "Supplier";
                worksheet.Cell(row, 4).Value = "Description";
                worksheet.Cell(row, 5).Value = "Unit";
                worksheet.Cell(row, 6).Value = "Current Quantity";
                worksheet.Cell(row, 7).Value = "Minimum Threshold";
                worksheet.Cell(row, 8).Value = "Missing Quantity";
                worksheet.Cell(row, 9).Value = "Suggested Reorder Quantity";

                worksheet.Range(row, 1, row, 9).Style.Font.Bold = true;
                worksheet.Range(row, 1, row, 9).Style.Fill.BackgroundColor = XLColor.LightGray;
                row++;

                if (shortageItems.Count == 0)
                {
                    worksheet.Cell(row, 1).Value = "No shortage items found.";
                }
                else
                {
                    var groupedByCategory = shortageItems
                        .GroupBy(i => categoryMap.TryGetValue(i.CategoryId, out var catName) ? catName : "Unknown")
                        .OrderBy(g => g.Key)
                        .ToList();

                    foreach (var categoryGroup in groupedByCategory)
                    {
                        // Category row: only category column filled
                        worksheet.Cell(row, 1).Value = categoryGroup.Key;
                        worksheet.Cell(row, 1).Style.Font.Bold = true;
                        worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;

                        worksheet.Range(row, 1, row, 9).Style.Fill.BackgroundColor = XLColor.LightGray;
                        row++;

                        foreach (var item in categoryGroup.OrderBy(i => i.Name).ThenBy(i => i.Supplier))
                        {
                            var missingQuantity = Math.Max(0, item.MinimumThreshold - item.CurrentQuantity);
                            var suggestedReorderQuantity = missingQuantity;

                            // Leave Category column empty in item rows
                            worksheet.Cell(row, 2).Value = item.Name;
                            worksheet.Cell(row, 3).Value = item.Supplier;
                            worksheet.Cell(row, 4).Value = item.Description;
                            worksheet.Cell(row, 5).Value = item.Unit;
                            worksheet.Cell(row, 6).Value = item.CurrentQuantity;
                            worksheet.Cell(row, 7).Value = item.MinimumThreshold;
                            worksheet.Cell(row, 8).Value = missingQuantity;
                            worksheet.Cell(row, 9).Value = suggestedReorderQuantity;

                            worksheet.Range(row, 1, row, 9).Style.Fill.BackgroundColor = XLColor.LightPink;
                            row++;
                        }

                        row++;
                    }
                }

                worksheet.Columns().AdjustToContents();

                // Optional second sheet: summary by supplier
                var summarySheet = workbook.Worksheets.Add("Summary By Supplier");
                int summaryRow = 1;

                summarySheet.Cell(summaryRow, 1).Value = "Supplier Summary";
                summarySheet.Cell(summaryRow, 1).Style.Font.Bold = true;
                summarySheet.Cell(summaryRow, 1).Style.Font.FontSize = 18;
                summaryRow += 2;

                summarySheet.Cell(summaryRow, 1).Value = "Supplier";
                summarySheet.Cell(summaryRow, 2).Value = "Items To Reorder";

                summarySheet.Range(summaryRow, 1, summaryRow, 2).Style.Font.Bold = true;
                summarySheet.Range(summaryRow, 1, summaryRow, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
                summaryRow++;

                var groupedBySupplier = shortageItems
                    .GroupBy(i => string.IsNullOrWhiteSpace(i.Supplier) ? "Unknown Supplier" : i.Supplier)
                    .OrderBy(g => g.Key)
                    .ToList();

                if (groupedBySupplier.Count == 0)
                {
                    summarySheet.Cell(summaryRow, 1).Value = "No shortage items found.";
                }
                else
                {
                    foreach (var group in groupedBySupplier)
                    {
                        summarySheet.Cell(summaryRow, 1).Value = group.Key;
                        summarySheet.Cell(summaryRow, 2).Value = group.Count();
                        summaryRow++;
                    }
                }

                summarySheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var safeOrganizationName = string.IsNullOrWhiteSpace(organization.Name)
                    ? "Organization"
                    : string.Concat(organization.Name.Split(Path.GetInvalidFileNameChars()));

                var fileName = $"ReorderReport_{safeOrganizationName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to generate shortage report: {ex.Message}");
            }
        }
    }
}