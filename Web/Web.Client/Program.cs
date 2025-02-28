using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Web.Client.Services;
using Web.Client.Providers;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure HttpClient pour appeler votre API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5269/") });

// Ajoutez Blazored.LocalStorage
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();


// Enregistrez AuthService et le AuthenticationStateProvider personnalisé
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<IClipboardService, ClipboardService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider,JwtAuthenticationStateProvider>();


await builder.Build().RunAsync();
