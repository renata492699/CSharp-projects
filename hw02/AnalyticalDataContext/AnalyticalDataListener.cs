using HW02.AnalyticalDataContext.DB;
using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.Enum;
using HW02.InputOutput.Printer;

namespace HW02.AnalyticalDataContext;

public class AnalyticalDataListener
{
    private readonly AnalyticalDBContext _analyticalDbContext;

    public AnalyticalDataListener(AnalyticalDBContext aContext)
    {
        _analyticalDbContext = aContext;
    }

    private static void ChangeCategory(Operation op, Entity entity, List<AnalyticalData> data)
    {
        if (op == Operation.Add)
        {
            data.Add(new AnalyticalData(entity.Id, entity.Name, 0));
        }
        else if (op == Operation.Delete)
        {
            var dataToAlter = data.Where(d => d.CategoryId == entity.CategoryId).First();
            data.Remove(dataToAlter);
        }
    }

    private static void ChangeProduct(Operation op, Entity entity, List<AnalyticalData> data)
    {
        var dataToChange = data.Where(d => d.CategoryId == entity.CategoryId).First();

        if (op == Operation.Add)
        {
            dataToChange.Count++;
        }
        else if (op == Operation.Delete)
        {
            dataToChange.Count--;
        }
    }

    public void SaveAnalyticalData(Operation op, EntityType type, Entity? entity, Exception? exception)
    {
        if (op == Operation.Get || exception != null)
        {
            return;
        }

        try
        {
            var data = _analyticalDbContext.ReadAnalyticalData();

            if (type == EntityType.Category)
            {
                ChangeCategory(op, entity, data);
            }
            else if (type == EntityType.Product)
            {
                ChangeProduct(op, entity, data);
            }

            _analyticalDbContext.SaveAnalyticalData(data);
        }
        catch (Exception ex)
        {
            MessagePrinter.AnalyticalDataFailure(ex.Message);
        }
    }
}