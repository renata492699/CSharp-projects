using HW02.InputOutput.Printer;

namespace HW02.Helpers;

public static class ParseHelper
{
    public static bool ValidNumberOfParams(int num, int input)
    {
        if (input == num)
        {
            return true;
        }
        MessagePrinter.IncorrectNumberOfParams();
        return false;
    }
}