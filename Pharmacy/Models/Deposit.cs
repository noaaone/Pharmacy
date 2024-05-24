namespace Pharmacy_.Models;

public class Deposit
{
    public int Id { get; set; }
    public double SumOfDep { get; set;}
    public DateTime Date { get; set; }
    public int UserId { get; set; }

    public Deposit(int id, double sumOfDep, DateTime date, int userId)
    {
        Id = id;
        SumOfDep = sumOfDep;
        Date = date;
        UserId = userId;
    }
}