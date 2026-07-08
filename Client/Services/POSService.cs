using System.Net.Http.Json;
using BIRPOSSystem.Shared;

namespace BIRPOSSystem.Client.Services;

public interface IPOSService
{
    Task<PosCatalogDto?> GetCatalogAsync();
    Task<HttpResponseMessage> CreateSaleAsync(CreateSaleRequest request);
}

public sealed class POSService(HttpClient http) : IPOSService
{
    public Task<PosCatalogDto?> GetCatalogAsync() =>
        http.GetFromJsonAsync<PosCatalogDto>("/api/pos/catalog");

    public Task<HttpResponseMessage> CreateSaleAsync(CreateSaleRequest request) =>
        http.PostAsJsonAsync("/api/sales", request);
}
