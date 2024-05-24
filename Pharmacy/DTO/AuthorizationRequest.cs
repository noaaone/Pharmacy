namespace Pharmacy_.DTO;

public class AuthorizationRequest
{
    public string Login { get; set; }
    public string Password { get; set; }

    public AuthorizationRequest(string login, string password)
    {
        Login = login;
        Password = password;
    }
}