namespace HW02.AnalyticalDataContext.DB;

public class AnalyticalData
{
    public AnalyticalData(int CategoryId, string CategoryName, int Count)
    {
        this.CategoryId = CategoryId;
        this.CategoryName = CategoryName;
        this.Count = Count;
    }

    public int CategoryId { get; }
    private string CategoryName { get; }
    public int Count { get; set; }
}