namespace Pharmacy_.Models;

public class Item
{
    public int Id { get; set; }
    public string ItemName { get; set; }
    public int ManufacturerId { get; set; }
    public double Price { get; set; }
    
    public int Quantity { get; set; }
    public string ExpertView { get; set; }
    public double ExpertViewPrice { get; set; }

    public Item(int id, string itemName, int manufacturerId, double price, int quantity, string expertView, double expertViewPrice)
    {
        Id = id;
        ItemName = itemName;
        ManufacturerId = manufacturerId;
        Price = price;
        Quantity = quantity;
        ExpertView = expertView;
        ExpertViewPrice = expertViewPrice;
    }
}