
namespace Pharmacy_.DTO;

public class AddManufacturerRequest
{
    public string Name { get; set; }
    public string Country { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

    public AddManufacturerRequest(string name, string country, string phone, string email)
    {
        Name = name;
        Country = country;
        Phone = phone;
        Email = email;
    }
}