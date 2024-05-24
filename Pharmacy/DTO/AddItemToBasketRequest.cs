namespace Pharmacy_.DTO;

public class AddItemToBasketRequest
{
    public int ItemId { get; set; }
    public int UserId { get; set; }
    public int Quantity { get; set; }

    public AddItemToBasketRequest(int itemId, int userId, int quantity)
    {
        ItemId = itemId;
        UserId = userId;
        Quantity = quantity;
    }
}