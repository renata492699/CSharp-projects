using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.Enum;
using HW02.InputOutput.Printer;

namespace HW02.BussinessContext.Services;

public class CategoryService : Service
{
    protected override EntityType EntityType => EntityType.Category;

    protected override void AddEntity(Entity entity)
    {
        var categories = CategoryDBContext.ReadCategories();
        categories.Add((Category)entity);
        CategoryDBContext.SaveCategories(categories);
    }

    protected override void DeleteEntity(int id, out Entity deletedEntity)
    {
        var categories = CategoryDBContext.ReadCategories();

        if (categories.Where(c => c.Id == id).Count() == 0)
        {
            throw new Exception("Category with this id does not exist.");
        }

        var categoryToDelete = categories.Where(c => c.Id == id).First();
        categories.Remove(categoryToDelete);

        //delete products in deleted category
        var products = ProductDBContext.ReadProducts();
        var productsToKeep = products.Where(p => p.CategoryId != id).ToList();

        CategoryDBContext.SaveCategories(categories);
        ProductDBContext.SaveProducts(productsToKeep);
        deletedEntity = categoryToDelete;
    }

    protected override void PrintTable()
    {
        var categories = CategoryDBContext.ReadCategories();
        TablePrinter.PrintCategory(categories);
    }
}