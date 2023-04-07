using HW02.AnalyticalDataContext;
using HW02.AnalyticalDataContext.DB;
using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.Enum;
using HW02.BussinessContext.FileDatabase;
using HW02.InputOutput.Printer;
using HW02.LoggerContext.DB;

namespace HW02.BussinessContext.Services;

public abstract class Service
{
    public Action<Operation, EntityType, Entity?, Exception?> SendData;

    protected Service()
    {
        AvailableId = 0;

        CategoryDBContext = new CategoryDBContext();
        ProductDBContext = new ProductDBContext(CategoryDBContext);

        var loggerListener = new LoggerListener(new LoggerDBContext());
        var analyticalDataListener = new AnalyticalDataListener(new AnalyticalDBContext());

        SendData += loggerListener.SaveLog;
        SendData += analyticalDataListener.SaveAnalyticalData;
    }

    protected abstract EntityType EntityType { get; }
    protected CategoryDBContext CategoryDBContext { get; }
    protected ProductDBContext ProductDBContext { get; }
    private static int AvailableId { get; set; }

    protected abstract void AddEntity(Entity entity);
    protected abstract void PrintTable();
    protected abstract void DeleteEntity(int id, out Entity deletedEntity);

    public bool Add(string name)
    {
        Entity entity = new Category(AvailableId, name);
        try
        {
            AddEntity(entity);
            SendData(Operation.Add, EntityType, entity, null);
            AvailableId++;
            return true;
        }
        catch (Exception ex)
        {
            SendData(Operation.Add, EntityType, null, ex);

            MessagePrinter.AddEntityFailure(EntityType, ex.Message);
            return false;
        }
    }

    public void Add(string name, int categoryId, int price)
    {
        Entity entity = new Product(AvailableId, name, categoryId, price);
        try
        {
            AddEntity(entity);
            SendData(Operation.Add, EntityType, entity, null);
            AvailableId++;
        }
        catch (Exception ex)
        {
            SendData(Operation.Add, EntityType, null, ex);

            MessagePrinter.AddEntityFailure(EntityType, ex.Message);
        }
    }

    public void Delete(int id)
    {
        try
        {
            DeleteEntity(id, out var deletedEntity);
            SendData(Operation.Delete, EntityType, deletedEntity, null);
        }
        catch (Exception ex)
        {
            SendData(Operation.Delete, EntityType, null, ex);
            MessagePrinter.DeleteEntityFailure(EntityType, ex.Message);
        }
    }

    public void ListAll()
    {
        try
        {
            PrintTable();
            SendData(Operation.Get, EntityType, null, null);
        }
        catch (Exception ex)
        {
            MessagePrinter.ReadEntitiesFailure(EntityType, ex.Message);
            SendData(Operation.Get, EntityType, null, ex);
        }
    }

    public void GetProductByCategoryId(int categoryId)
    {
        try
        {
            var products = ProductDBContext.ReadProducts();
            var categories = CategoryDBContext.ReadCategories();

            if (categories.Where(c => c.Id == categoryId).Count() == 0)
            {
                throw new Exception("Category with this id does not exist.");
            }

            var productsInCategory = products.Where(p => p.CategoryId == categoryId).ToList();

            TablePrinter.PrintProducts(productsInCategory);
            SendData(Operation.Get, EntityType.Product, null, null);
        }
        catch (Exception ex)
        {
            MessagePrinter.ReadEntitiesFailure(EntityType, ex.Message);
            SendData(Operation.Get, EntityType.Product, null, ex);
        }
    }
}