using System.Net.Http.Json;
using ClientApp.Models;

namespace ClientApp.Services;

// All calls to the back end go through this service: one place for the URL, error handling and caching.
public class ProductService(HttpClient http)
{
    private Product[]? cache;
    private DateTime cachedAt;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    public async Task<Product[]> GetProductsAsync(bool refresh = false)
    {
        // Performance: reuse the last response for a minute instead of calling the API on every visit.
        if (!refresh && cache is not null && DateTime.UtcNow - cachedAt < CacheDuration)
        {
            return cache;
        }

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        try
        {
            cache = await http.GetFromJsonAsync<Product[]>("api/productlist", cts.Token) ?? [];
            cachedAt = DateTime.UtcNow;
            return cache;
        }
        catch (OperationCanceledException)
        {
            throw new ProductServiceException("The server did not respond within 10 seconds.");
        }
        catch (HttpRequestException ex)
        {
            throw new ProductServiceException($"Could not reach the API ({ex.StatusCode?.ToString() ?? ex.Message}). Is ServerApp running?");
        }
        catch (System.Text.Json.JsonException)
        {
            throw new ProductServiceException("The API returned data in an unexpected format.");
        }
    }
}

public class ProductServiceException(string message) : Exception(message);
