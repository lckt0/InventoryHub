using System.Text.Json;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// The Blazor client runs on a different origin, so the API must allow it explicitly (CORS).
// Origins come from configuration so the deployed client URL can be added without code changes.
var clientOrigins = builder.Configuration.GetSection("ClientOrigins").Get<string[]>()
                    ?? new[] { "http://localhost:5108", "https://localhost:7082" };
builder.Services.AddCors(options =>
    options.AddPolicy("ClientApp", policy => policy.WithOrigins(clientOrigins).AllowAnyHeader().WithMethods("GET")));

// Performance: compress JSON responses and keep the product list in memory between requests.
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.AddMemoryCache();

// Consistent JSON: camelCase property names, no nulls.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

app.UseResponseCompression();
app.UseCors("ClientApp");

app.MapGet("/api/productlist", (IMemoryCache cache) =>
{
    var products = cache.GetOrCreate("productlist", entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        return ProductCatalog.Load();
    })!;
    return Results.Ok(products);
})
.WithName("GetProductList");

app.MapGet("/api/productlist/{id:int}", (int id, IMemoryCache cache) =>
{
    var products = cache.GetOrCreate("productlist", _ => ProductCatalog.Load())!;
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is null
        ? Results.NotFound(new { error = $"Product {id} was not found." })
        : Results.Ok(product);
});

app.Run();

// JSON contract shared with the client: a product with a nested category object.
public record Category(int Id, string Name);

public record Product(int Id, string Name, decimal Price, int Stock, Category Category);

static class ProductCatalog
{
    private static readonly Category Electronics = new(101, "Electronics");
    private static readonly Category Accessories = new(102, "Accessories");

    public static IReadOnlyList<Product> Load() => new List<Product>
    {
        new(1, "Laptop", 1200.50m, 25, Electronics),
        new(2, "Headphones", 50.00m, 100, Accessories),
        new(3, "Monitor", 219.99m, 40, Electronics),
        new(4, "USB-C Hub", 34.90m, 0, Accessories),
    };
}
