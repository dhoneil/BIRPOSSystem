using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BIRPOSSystem.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPOSService, POSService>();
builder.Services.AddScoped<IBIRReportService, BIRReportService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

await builder.Build().RunAsync();
