using HW02.BussinessContext.Services;
using HW02.Helpers;
using HW02.InputOutput.Printer;

namespace HW02.InputOutput;

public class Parser
{
    public Parser(CategoryService categoryService, ProductService productService)
    {
        CategoryService = categoryService;
        ProductService = productService;
    }

    private ProductService ProductService { get; }
    private CategoryService CategoryService { get; }

    public void ParseInput(string input)
    {
        var split = input.Split(' ');
        if (split.Length == 0)
        {
            return;
        }
        var operation = split[0].Split('-');

        switch (operation[0])
        {
            case "add":
                InputAdd(split);
                break;
            case "delete":
                InputDelete(split);
                break;
            case "list":
                InputList(split);
                break;
            case "get":
                InputGet(split);
                break;
            default:
                MessagePrinter.IncorrectFormat();
                break;
        }
    }

    private void InputGet(string[] split)
    {
        if (!ParseHelper.ValidNumberOfParams(2, split.Length))
        {
            return;
        }
        if (decimal.TryParse(split[1], out var id))
        {
            ProductService.GetProductByCategoryId(decimal.ToInt32(id));
            return;
        }

        MessagePrinter.IncorrectFormat();
    }

    private void InputList(string[] split)
    {
        if (!ParseHelper.ValidNumberOfParams(1, split.Length))
        {
            return;
        }

        if (split[0] == "list-products")
        {
            ProductService.ListAll();
        }
        else if (split[0] == "list-categories")
        {
            CategoryService.ListAll();
        }
    }

    private void InputDelete(string[] split)
    {
        if (!ParseHelper.ValidNumberOfParams(2, split.Length))
        {
            return;
        }

        if (decimal.TryParse(split[1], out var id))
        {
            if (split[0] == "delete-product")
            {
                ProductService.Delete(decimal.ToInt32(id));
            }
            else if (split[0] == "delete-category")
            {
                CategoryService.Delete(decimal.ToInt32(id));
            }
            return;
        }

        MessagePrinter.IncorrectFormat();
    }

    private void InputAdd(string[] split)
    {
        if (split[0] == "add-product")
        {
            if (!ParseHelper.ValidNumberOfParams(4, split.Length))
            {
                return;
            }

            if (decimal.TryParse(split[2], out var id) &&
                decimal.TryParse(split[3], out var price))
            {
                ProductService.Add(split[1], decimal.ToInt32(id), decimal.ToInt32(price));
                return;
            }
        }
        else if (split[0] == "add-category")
        {
            if (!ParseHelper.ValidNumberOfParams(2, split.Length))
            {
                return;
            }
            CategoryService.Add(split[1]);
            return;
        }

        MessagePrinter.IncorrectFormat();
    }
}