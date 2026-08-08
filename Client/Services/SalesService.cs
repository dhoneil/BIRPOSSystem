using System.Net.Http.Json;
using BIRPOSSystem.Shared;

namespace BIRPOSSystem.Client.Services;

public interface ISalesService
{
    Task<List<SalesHistoryDto>> GetHistoryAsync();
}

public sealed class SalesService(HttpClient http) : ISalesService
{
    public async Task<List<SalesHistoryDto>> GetHistoryAsync() =>
        await http.GetFromJsonAsync<List<SalesHistoryDto>>("/api/sales/history") ?? [];
}
