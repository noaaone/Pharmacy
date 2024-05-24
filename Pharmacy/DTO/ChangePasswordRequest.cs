namespace Pharmacy_.DTO;

public class ChangePasswordRequest
{
    public int Id { get; set; }
    public string Password { get; set; }
    public string NewPassword { get; set; }

    public ChangePasswordRequest(int id, string password, string newPassword)
    {
        Id = id;
        Password = password;
        NewPassword = newPassword;
    }
}