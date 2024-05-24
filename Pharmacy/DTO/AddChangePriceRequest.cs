namespace Pharmacy_.DTO;

public class AddChangePriceRequest
{
    public int Id { get; set; }
    public double Price { get; set; }

    public AddChangePriceRequest(int id, double price)
    {
        Id = id;
        Price = price;
    }
}