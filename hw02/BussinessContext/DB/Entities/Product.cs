namespace HW02.BussinessContext.DB.Entities;

public class Product : Entity
{
    public Product(int id, string name, int categoryId, int price) : base(id, name, categoryId)
    {
        Price = price;
    }

    public int Price { get; }
}