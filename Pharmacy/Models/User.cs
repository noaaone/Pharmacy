namespace Pharmacy_.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public double Balance { get; set; }
    public string Salt { get; set; }
    public int Role { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsDeleted { get; set; }


public User(int id, string login, string password, double balance, string salt, int role, bool isBlocked, bool isDeleted)
    {
        Id = id;
        Login = login;
        Password = password;
        Balance = balance;
        Salt = salt;
        Role = role;
        IsBlocked = isBlocked;
        IsDeleted = isDeleted;
    }

public User()
{
    
}

    public string toString()
    {
        return "Id = " + Id +
               "Login = " + Login +
               "Password = " + Password +
               "Balance = " + Balance +
               "Salt = " + Salt +
               "Role = " + Role +
               "Is blocked = " + IsBlocked +
               "Is deleted = " + IsDeleted;
    }
}