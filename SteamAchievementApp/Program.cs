using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SteamAchievementApp;
using SteamAchievementApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// The base address is not directly needed for external calls, 
// but we register HttpClient to be able to perform Http operations.
builder.Services.AddScoped(sp => new HttpClient());

// Register our AchievementService
builder.Services.AddScoped<AchievementService>();

await builder.Build().RunAsync();
