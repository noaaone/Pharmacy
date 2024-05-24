namespace Pharmacy_.Models;

public class Manufacturer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

    public Manufacturer(int id, string name, string country, string phone, string email)
    {
        Id = id;
        Name = name;
        Country = country;
        Phone = phone;
        Email = email;
    }
}