using System.Text;
using HW02.Helpers;

namespace HW02.LoggerContext.DB;

public class LoggerDBContext
{
    private readonly string _filePath;

    private readonly string[] _paths =
        { @"c:\", "Users", "urban", "source", "repos", "hw02", "LoggerContext", "DB", "Storage", "Log.txt" };

    public LoggerDBContext()
    {
        _filePath = Path.Combine(_paths);
        FileHelper.CreateFile(_filePath);
    }


    public void WriteLog(StringBuilder log)
    {
        using var sw = File.AppendText(_filePath);
        sw.WriteLine(log);
    }
}