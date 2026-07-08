using BIRPOSSystem.Data;
using BIRPOSSystem.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BIRPOSSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/product-categories")]
public sealed class ProductCategoryController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<ProductCategoryDto>> GetCategoriesAsync() =>
        await db.ProductCategories
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new ProductCategoryDto(x.Id, x.Name))
            .ToListAsync();
}
