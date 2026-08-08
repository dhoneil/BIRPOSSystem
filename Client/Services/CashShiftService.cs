using System.Net.Http.Json;
using BIRPOSSystem.Shared;

namespace BIRPOSSystem.Client.Services;

public interface ICashShiftService
{
    Task<CashShiftWorkspaceDto?> GetWorkspaceAsync();
    Task<HttpResponseMessage> OpenAsync(OpenCashShiftRequest request);
    Task<HttpResponseMessage> AddMovementAsync(Guid shiftId, AddCashMovementRequest request);
    Task<HttpResponseMessage> CloseAsync(Guid shiftId, CloseCashShiftRequest request);
}

public sealed class CashShiftService(HttpClient http) : ICashShiftService
{
    public Task<CashShiftWorkspaceDto?> GetWorkspaceAsync() =>
        http.GetFromJsonAsync<CashShiftWorkspaceDto>("/api/cash-shifts/workspace");

    public Task<HttpResponseMessage> OpenAsync(OpenCashShiftRequest request) =>
        http.PostAsJsonAsync("/api/cash-shifts/open", request);

    public Task<HttpResponseMessage> AddMovementAsync(Guid shiftId, AddCashMovementRequest request) =>
        http.PostAsJsonAsync($"/api/cash-shifts/{shiftId}/movements", request);

    public Task<HttpResponseMessage> CloseAsync(Guid shiftId, CloseCashShiftRequest request) =>
        http.PostAsJsonAsync($"/api/cash-shifts/{shiftId}/close", request);
}
