namespace Pharmacy_.Models;

public class Purchase
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }

    public Purchase(int id, double price, int userId, int itemId)
    {
        Id = id;
        Price = price;
        UserId = userId;
        ItemId = itemId;
    }
}