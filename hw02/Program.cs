using HW02.BussinessContext.Services;
using HW02.InputOutput;

namespace HW02;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello Ehop!");
        var categoryService = new CategoryService();
        var productService = new ProductService();

        Seeding.Seeding.Seed(categoryService, productService);
        var parser = new Parser(categoryService, productService);

        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput != null)
            {
                parser.ParseInput(userInput);
            }
        }
    }
}