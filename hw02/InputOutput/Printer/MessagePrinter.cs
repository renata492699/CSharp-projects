using HW02.BussinessContext.Enum;

namespace HW02.InputOutput.Printer;

public static class MessagePrinter
{
    public static void IncorrectNumberOfParams()
    {
        Console.WriteLine("Incorrect number of parameters.");
    }

    public static void IncorrectFormat()
    {
        Console.WriteLine("Incorrect format of input.");
    }

    public static void LogDataFailure(string message)
    {
        Console.WriteLine("Data were not logged. {0}", message);
    }

    public static void AddEntityFailure(EntityType type, string message)
    {
        Console.WriteLine("{0} was not added. {1}", type, message);
    }

    public static void DeleteEntityFailure(EntityType type, string message)
    {
        Console.WriteLine("{0} was not deleted. {1}", type, message);
    }

    public static void ReadEntitiesFailure(EntityType type, string message)
    {
        Console.WriteLine("Read entities of type {0} failed. {1}", type, message);
    }

    public static void AnalyticalDataFailure(string message)
    {
        Console.WriteLine("Analytical data was not saved. {0}", message);
    }
}