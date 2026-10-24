using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ClientApp;
using ClientApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// The API address comes from wwwroot/appsettings.json, so it can change per environment without a rebuild.
var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5178/";
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBase) });
builder.Services.AddScoped<ProductService>();

await builder.Build().RunAsync();
