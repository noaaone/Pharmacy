namespace Pharmacy_.DTO;

public class AddItemRequest
{
    public string ItemName { get; set; }
    public int ManufacturerId { get; set; }
    public double Price { get; set; }
    public string ExpertView { get; set; }
    public double ExpertViewPrice { get; set; }

    public AddItemRequest(string itemName, int manufacturerId, double price, string expertView, double expertViewPrice)
    {
        ItemName = itemName;
        ManufacturerId = manufacturerId;
        Price = price;
        ExpertView = expertView;
        ExpertViewPrice = expertViewPrice;
    }
}