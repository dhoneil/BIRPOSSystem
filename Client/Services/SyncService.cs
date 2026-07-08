using System.Net.Http.Json;
using BIRPOSSystem.Shared;

namespace BIRPOSSystem.Client.Services;

public interface ISyncService
{
    Task<SyncStatusDto?> GetStatusAsync();
    Task<HttpResponseMessage> UploadAsync();
}

public sealed class SyncService(HttpClient http) : ISyncService
{
    public Task<SyncStatusDto?> GetStatusAsync() =>
        http.GetFromJsonAsync<SyncStatusDto>("/api/sync/status");

    public Task<HttpResponseMessage> UploadAsync() =>
        http.PostAsJsonAsync("/api/sync/upload", new { });
}
