using System.Net.Http.Json;
using BIRPOSSystem.Shared;

namespace BIRPOSSystem.Client.Services;

public interface IProductService
{
    Task<List<ProductCategoryDto>> GetCategoriesAsync();
    Task<List<ProductDto>> GetProductsAsync();
    Task<HttpResponseMessage> AddAsync(UpsertProductRequest request);
}

public sealed class ProductService(HttpClient http) : IProductService
{
    public async Task<List<ProductCategoryDto>> GetCategoriesAsync() =>
        await http.GetFromJsonAsync<List<ProductCategoryDto>>("/api/product-categories") ?? [];

    public async Task<List<ProductDto>> GetProductsAsync() =>
        await http.GetFromJsonAsync<List<ProductDto>>("/api/products") ?? [];

    public Task<HttpResponseMessage> AddAsync(UpsertProductRequest request) =>
        http.PostAsJsonAsync("/api/products", request);
}
