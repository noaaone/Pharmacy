namespace Pharmacy_.Models;

public class RecentPasswords
{
    private int Id { get; set; }
    public string Password { get; set; }
    private int UserId { get; set; }
   
    public RecentPasswords(int id, string password, int userId)
    {
        Id = id;
        Password = password;
        UserId = userId;
    }
    
}