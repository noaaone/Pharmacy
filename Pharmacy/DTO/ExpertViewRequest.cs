namespace Pharmacy_.DTO;

public class ExpertViewRequest
{
    public int UserId { get; set; }
    public int ItemId { get; set; }

    public ExpertViewRequest(int userId, int itemId)
    {
        UserId = userId;
        ItemId = itemId;
    }
}