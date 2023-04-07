using HW02.BussinessContext.DB.Entities;

namespace HW02.InputOutput.Printer;

public static class TablePrinter
{
    public static void PrintProducts(List<Product> products)
    {
        Console.WriteLine();
        Console.WriteLine("{0, -3} | {1, -10} | {2, -10} | {3, -5} ", "Id", "Name", "CategoryId", "Price");
        Console.WriteLine(new string('-', 38));
        foreach (var product in products)
        {
            Console.WriteLine("{0, -3} | {1, -10} | {2, -10} | {3, -5} ", product.Id, product.Name, 
                product.CategoryId, product.Price);
        }
        Console.WriteLine();
    }

    public static void PrintCategory(List<Category> categories)
    {
        Console.WriteLine();
        Console.WriteLine("{0, -3} | {1, -10} ", "Id", "Name");
        Console.WriteLine(new string('-', 13));
        foreach (var category in categories)
        {
            Console.WriteLine("{0, -3} | {1, -10} ", category.Id, category.Name);
        }
        Console.WriteLine();
    }
}