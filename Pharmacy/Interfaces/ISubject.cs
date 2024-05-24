namespace Pharmacy_.Interfaces;

public interface ISubject
{
    void Subscribe(int userId,int itemId);
    void Unsubscribe(int userId,int itemId);
    void Notify(int userId,string notification,DateTime date);
}