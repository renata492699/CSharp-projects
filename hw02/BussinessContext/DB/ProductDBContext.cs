using System.Text.Json;
using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.FileDatabase;
using HW02.Helpers;

namespace HW02.BussinessContext;

public class ProductDBContext
{
    private readonly CategoryDBContext _categoryDBContext;
    private readonly string _filePath;

    private readonly string[] _paths =
        { @"c:\", "Users", "urban", "source", "repos", "hw02", "BussinessContext", "DB", "Storage", "Products.json" };

    public ProductDBContext(CategoryDBContext categoryDBcontext)
    {
        _categoryDBContext = categoryDBcontext;

        _filePath = Path.Combine(_paths);
        FileHelper.CreateFile(_filePath);
    }

    public void SaveProducts(IEnumerable<Product> products)
    {
        if (products.Select(p => p.Id).Distinct().Count() != products.Count())
        {
            var duplicitProduct = products.GroupBy(p => p.Id).Select(g => new { id = g.Key, count = g.Count() })
                .Where(g => g.count > 1).First();
            var product = products.Where(p => p.Id == duplicitProduct.id).First();
            throw new EntityWithSameIdAlreadyExistException<Product>(product);
        }

        var categories = _categoryDBContext.ReadCategories();

        var productIds = products.Select(p => p.CategoryId);
        var catIds = categories.Select(c => c.Id);

        if (productIds.Any(p => !catIds.Contains(p)))
        {
            throw new Exception("The category for the new product does not exist.");
        }

        var jsonString = JsonSerializer.Serialize(products);
        File.Delete(_filePath);
        using var outputFile = new StreamWriter(_filePath);
        outputFile.WriteLine(jsonString);
    }

    public List<Product> ReadProducts()
    {
        string? line;
        using (var inputFile = new StreamReader(_filePath))
        {
            line = inputFile.ReadLine();
        }

        if (line == null)
        {
            return new List<Product>();
        }

        var model = JsonSerializer.Deserialize<List<Product>>(line);

        return model;
    }
}