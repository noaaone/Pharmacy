namespace Pharmacy_.DTO;

public class AddCreditCardRequest
{
    public int Id { get; set; }
    public string CardNumber { get; set; }
    public string Name { get; set; }
    public string GoodTrue { get; set; }
    public string Cvv { get; set; }

    public AddCreditCardRequest(int id, string cardNumber, string name, string goodTrue, string cvv)
    {
        Id = id;
        CardNumber = cardNumber;
        Name = name;
        GoodTrue = goodTrue;
        Cvv = cvv;
    }
}