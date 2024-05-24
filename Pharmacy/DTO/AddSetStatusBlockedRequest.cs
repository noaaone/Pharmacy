namespace Pharmacy_.DTO;

public class AddSetStatusBlockedRequest
{
    
    public int Id { get; set; }
    public bool IsBlocked { get; set; }

    public AddSetStatusBlockedRequest(int id, bool isBlocked)
    {
        Id = id;
        IsBlocked = isBlocked;
    }
}