namespace Pharmacy_.Models;

public class CreditCard
{
    public int Id { get; set; }
    public int CardNumber { get; set; }
    public string Name { get; set; }
    public string Date { get; set; }
    public int Cvv { get; set; }
    public int UserId { get; set; }

    public CreditCard(int id, int cardNumber, string name, string date, int cvv, int userId)
    {
        Id = id;
        CardNumber = cardNumber;
        Name = name;
        Date = date;
        Cvv = cvv;
        UserId = userId;
    }
}