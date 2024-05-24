namespace Pharmacy_.DTO;

public class AddSetRoleRequest
{
    public int Id { get; set; }
    public int Role { get; set; }

    public AddSetRoleRequest(int id, int role)
    {
        Id = id;
        Role = role;
    }
}