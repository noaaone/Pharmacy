namespace Pharmacy_.DTO;

public class EditExpertViewRequest
{
    public int ItemId { get; set; }
    public string NewView { get; set; }
    public double NewPriceOfView { get; set; }

    public EditExpertViewRequest(int itemId, string newView, double newPriceOfView)
    {
        ItemId = itemId;
        NewView = newView;
        NewPriceOfView = newPriceOfView;
    }
}