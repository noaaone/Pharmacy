namespace Pharmacy_.DTO;

public class AddSetStatusDeletedRequest
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }

    public AddSetStatusDeletedRequest(int id, bool isDeleted)
    {
        Id = id;
        IsDeleted = isDeleted;
    }
}