namespace Pharmacy_.Models;

public class Card
{
    public int Id { get; set; }
    public string CardNumber { get; set; }
    public string Name { get; set; }
    public string DateThru { get; set; }
    public int Cvv { get; set; }
    public int UserId { get; set; }

    public Card(int id, string cardNumber, string name, string dateThru, int cvv, int userId)
    {
        Id = id;
        CardNumber = cardNumber;
        Name = name;
        DateThru = dateThru;
        Cvv = cvv;
        UserId = userId;
    }
}