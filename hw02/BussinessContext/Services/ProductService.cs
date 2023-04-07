using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.Enum;
using HW02.InputOutput.Printer;

namespace HW02.BussinessContext.Services;

public class ProductService : Service
{
    protected override EntityType EntityType => EntityType.Product;

    protected override void AddEntity(Entity entity)
    {
        var products = ProductDBContext.ReadProducts();
        products.Add((Product)entity);
        ProductDBContext.SaveProducts(products);
    }

    protected override void DeleteEntity(int id, out Entity deletedEntity)
    {
        var products = ProductDBContext.ReadProducts();

        if (products.Where(p => p.Id == id).Count() == 0)
        {
            throw new Exception("Product with this id does not exist.");
        }

        var productToDelete = products.Where(p => p.Id == id).First();
        products.Remove(productToDelete);

        ProductDBContext.SaveProducts(products);
        deletedEntity = productToDelete;
    }

    protected override void PrintTable()
    {
        var products = ProductDBContext.ReadProducts();
        TablePrinter.PrintProducts(products);
    }
}