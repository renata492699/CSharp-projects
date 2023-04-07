using HW02.BussinessContext.Services;

namespace HW02.Seeding;

public static class Seeding
{
    public static void Seed(CategoryService cService, ProductService pService)
    {
        cService.Add("phone");
        cService.Add("pc");
        cService.Add("tv");
        pService.Add("iphone", 0, 4000);
        pService.Add("nokia", 0, 2000);
        pService.Add("hp", 1, 5000);
        pService.Add("sony", 2, 46000);
    }
}