namespace Pharmacy_.Models;

public class RecentPrices
{
    public int Id { get; set; }
    public double Price { get; set; }
    public DateTime Date { get; set; }
    public int ItemId { get; set; }

    public RecentPrices(int id, double price, DateTime date, int itemId)
    {
        Id = id;
        Price = price;
        Date = date;
        ItemId = itemId;
    }
}