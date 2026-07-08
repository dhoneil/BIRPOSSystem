using System.Net.Http.Json;
using BIRPOSSystem.Shared;

namespace BIRPOSSystem.Client.Services;

public interface IDashboardService
{
    Task<DashboardDto?> GetDashboardAsync();
}

public sealed class DashboardService(HttpClient http) : IDashboardService
{
    public Task<DashboardDto?> GetDashboardAsync() =>
        http.GetFromJsonAsync<DashboardDto>("/api/dashboard");
}
