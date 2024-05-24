namespace Pharmacy_.Models;

public class Basket
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }

    public Basket(int id, int quantity, double price, int userId, int itemId)
    {
        Id = id;
        Quantity = quantity;
        Price = price;
        UserId = userId;
        ItemId = itemId;
    }
}