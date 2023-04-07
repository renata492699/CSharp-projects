namespace HW02.BussinessContext.DB.Entities;

public class Entity
{
    public Entity(int id, string name, int categoryId)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
    }

    public int Id { get; }
    public string Name { get; }

    public int CategoryId { get; }
}