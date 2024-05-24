namespace Pharmacy_.DTO;

public class SubscriptionRequest
{
    public int UserId { get; set; }
    public int ItemId { get; set; }

    public SubscriptionRequest(int userId, int itemId)
    {
        UserId = userId;
        ItemId = itemId;
    }
}