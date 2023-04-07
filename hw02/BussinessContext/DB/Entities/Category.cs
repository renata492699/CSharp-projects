namespace HW02.BussinessContext.DB.Entities;

public class Category : Entity
{
    public Category(int id, string name) : base(id, name, id)
    {
    }
}