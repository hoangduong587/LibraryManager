using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LibraryManager.Client;
using LibraryManager.Client.Services;
using LibraryManager.Client.Auth;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API Base URL
var apiBaseUrl = "http://localhost:5030";

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

// ⭐ Custom JWT Handler (attaches Authorization: Bearer <token>)
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();

// ⭐ Authorized HttpClient (uses JWT handler)
builder.Services.AddHttpClient("AuthorizedClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

// ⭐ Default HttpClient = AuthorizedClient
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthorizedClient")
);

// ⭐ BookService
builder.Services.AddScoped<IBookService, BookService>();

await builder.Build().RunAsync();
