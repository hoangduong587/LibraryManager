using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LibraryManager.Client;
using LibraryManager.Client.Services;
using LibraryManager.Client.Auth;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API Base URL
var apiBaseUrl = "http://localhost:5030";

// HttpClient
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(apiBaseUrl) }
);

// ⭐ LocalStorage
builder.Services.AddBlazoredLocalStorage();

// ⭐ Authentication Core
builder.Services.AddAuthorizationCore();

// ⭐ Register CustomAuthStateProvider
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>()
);

// ⭐ AuthService
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();
