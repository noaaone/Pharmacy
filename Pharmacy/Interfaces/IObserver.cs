namespace Pharmacy_.Interfaces;

public interface IObserver
{
    public void ChangePrice(int itemId, double price);
}