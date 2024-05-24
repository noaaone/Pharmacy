namespace Pharmacy_.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderInfo { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; }
    public DateTime Date { get; set; }
    public double FullPrice { get; set; }

    public Order(int id, string orderInfo, int userId, string status, DateTime date, double fullPrice)
    {
        Id = id;
        OrderInfo = orderInfo;
        UserId = userId;
        Status = status;
        Date = date;
        FullPrice = fullPrice;

    }
    
}