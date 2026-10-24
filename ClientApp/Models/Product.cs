namespace ClientApp.Models;

// Mirrors the JSON returned by ServerApp's /api/productlist (camelCase is matched case-insensitively).
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Category? Category { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
