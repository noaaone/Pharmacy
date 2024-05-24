namespace Pharmacy_.DTO;

public class ReplenishBalanceRequest
{
    public int Id { get; set; }
    public string SumOfDep { get; set; }

    public ReplenishBalanceRequest(int id, string sumOfDep)
    {
        Id = id;
        SumOfDep = sumOfDep;
    }
}