namespace Pharmacy_.DTO;

public class RegisterRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string PasswordRepeat { get; set; }

    public RegisterRequest(string login, string password, string passwordRepeat)
    {
        Login = login;
        Password = password;
        PasswordRepeat = passwordRepeat;
    }
}