using System.Net.Http.Json;
using BIRPOSSystem.Shared;

namespace BIRPOSSystem.Client.Services;

public interface IBIRReportService
{
    Task<ZReadingDto?> GetTodayZReadingAsync();
}

public sealed class BIRReportService(HttpClient http) : IBIRReportService
{
    public Task<ZReadingDto?> GetTodayZReadingAsync() =>
        http.GetFromJsonAsync<ZReadingDto>("/api/reports/z-reading/today");
}
