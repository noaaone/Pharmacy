namespace Pharmacy_.DTO;

public class EditOrderStatusRequest
{
    public int OrderId { get; set; }
    public string Status { get; set; }

    public EditOrderStatusRequest(int orderId, string status)
    {
        OrderId = orderId;
        Status = status;
    }
}