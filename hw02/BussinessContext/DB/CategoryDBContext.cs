using System.Text.Json;
using HW02.BussinessContext.DB.Entities;
using HW02.Helpers;

namespace HW02.BussinessContext.FileDatabase;

public class CategoryDBContext
{
    private readonly string _filePath;
    // C:\Users\urban\source\repos\hw02

    private readonly string[] _paths =
        { @"c:\", "Users", "urban", "source", "repos", "hw02", "BussinessContext", "DB", "Storage", "Categories.json" };

    public CategoryDBContext()
    {
        _filePath = Path.Combine(_paths);
        FileHelper.CreateFile(_filePath);
    }

    public void SaveCategories(IEnumerable<Category> categories)
    {
        if (categories.Select(p => p.Id).Distinct().Count() != categories.Count())
        {
            var duplicitCategory = categories.GroupBy(c => c.Id).Select(g => new { id = g.Key, count = g.Count() })
                .Where(g => g.count > 1).First();
            var category = categories.Where(c => c.Id == duplicitCategory.id).First();
            throw new EntityWithSameIdAlreadyExistException<Category>(category);
        }

        var jsonString = JsonSerializer.Serialize(categories);
        File.Delete(_filePath);
        using var outputFile = new StreamWriter(_filePath);
        outputFile.WriteLine(jsonString);
    }

    public List<Category> ReadCategories()
    {
        string? line;
        using (var inputFile = new StreamReader(_filePath))
        {
            line = inputFile.ReadLine();
        }

        if (line == null)
        {
            return new List<Category>();
        }

        var model = JsonSerializer.Deserialize<List<Category>>(line);
        return model;
    }
}