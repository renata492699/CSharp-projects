using System.Text.Json;
using HW02.Helpers;

namespace HW02.AnalyticalDataContext.DB;

public class AnalyticalDBContext
{
    private readonly string _filePath;

    private readonly string[] _paths =
    {
        @"c:\", "Users", "urban", "source", "repos", "hw02", "AnalyticalDataContext", "DB", "Storage",
        "AnalyticalData.json"
    };

    public AnalyticalDBContext()
    {
        _filePath = Path.Combine(_paths);
        FileHelper.CreateFile(_filePath);
    }

    // TODO: replace type List<object> in functions headers to the appropriate data model -> List<YourDataModel>
    public void SaveAnalyticalData(List<AnalyticalData> log)
    {
        var jsonString = JsonSerializer.Serialize(log);
        using var outputFile = new StreamWriter(_filePath);
        outputFile.WriteLine(jsonString);
    }

    public List<AnalyticalData> ReadAnalyticalData()
    {
        string? line;
        using (var inputFile = new StreamReader(_filePath))
        {
            line = inputFile.ReadLine();
        }

        if (line == null)
        {
            return new List<AnalyticalData>();
        }

        var model = JsonSerializer.Deserialize<List<AnalyticalData>>(line);
        return model;
    }
}