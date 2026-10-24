# Reflection: how Copilot assisted in building InventoryHub

**1. Generating and refining integration code.** I asked Copilot to create a minimal API endpoint `/api/productlist` in ServerApp and a `FetchProducts` page in the Blazor ClientApp that calls it. Its first version called `HttpClient` directly from the page with a hard-coded URL. I asked it to refine that: the call moved into a `ProductService`, and the API address now comes from `wwwroot/appsettings.json` (`ApiBaseUrl`), so the front end can point to a different back end without code changes.

**2. Debugging integration issues.** When the client first ran, the browser blocked the request because the two apps run on different origins. Copilot explained the CORS error and generated a named CORS policy in ServerApp that allows only the client's origins (read from configuration) and only GET requests. It also helped add proper error handling: timeouts, unreachable API and malformed JSON each show a clear message on the page instead of crashing the app, and a missing product id returns a 404 with a JSON error body.

**3. Creating JSON structures.** Copilot proposed a richer JSON contract than a flat list: every product has `id`, `name`, `price`, `stock` and a nested `category` object (`id`, `name`). The server serializes it with camelCase names, and the client has matching `Product` and `Category` models, so the nested data is shown in the table (category column, "Out of stock" when stock is 0).

**4. Optimizing performance.** Copilot suggested three optimizations, which I applied: the server caches the product list with `IMemoryCache` (5 minutes) instead of rebuilding it on every request; responses are compressed with Brotli/Gzip; and the client keeps the last response for one minute, with a Refresh button to force a reload. Rows use `@key` so Blazor re-renders only the rows that changed.

**What I learned.** Copilot was fastest at boilerplate (endpoints, models, CORS and compression setup) and at explaining errors. I still had to review its output: I kept configuration out of the code, narrowed the CORS policy, and tested every failure path myself.
