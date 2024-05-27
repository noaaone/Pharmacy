namespace Pharmacy_.DTO;

public class ChangeQuantityRequest
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    public ChangeQuantityRequest(int id, int quantity)
    {
        Id = id;
        Quantity = quantity;
    }
}