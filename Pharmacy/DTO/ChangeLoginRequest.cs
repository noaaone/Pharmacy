namespace Pharmacy_.DTO;

public class ChangeLoginRequest
{
    public int Id { get; set; }
    public string NewLogin { get; set; }

    public ChangeLoginRequest(int id, string newLogin)
    {
        Id = id;
        NewLogin = newLogin;
    }
}