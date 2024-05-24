namespace Pharmacy_.Models;

public class LoginHistory
{
    public int Id { get; set; }
    public string Ip { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }

    public LoginHistory(int id, string ip, DateTime date, int userId)
    {
        Id = id;
        Ip = ip;
        Date = date;
        UserId = userId;
    }
}