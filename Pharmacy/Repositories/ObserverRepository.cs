using Npgsql;
using Pharmacy_.Interfaces;

namespace Pharmacy_.Repositories;

public class ObserverRepository : IObserver
{
    public void ChangePrice(int itemId, double price)
    {
        const string connectionString = "Host=127.0.0.1;Port=5432;Database=kursach;Username=postgres;Password=postgres;";
        var connection = new NpgsqlConnection(connectionString);
        var sr = new SubjectRepository();
        ItemsRepository repository = new ItemsRepository();
        var sql = "UPDATE public.items SET price = @price WHERE id = @id";
        var command = new NpgsqlCommand(sql, connection);
        foreach (var subscriber in sr.GetSubscribesList(itemId))
        {
            sr.Notify(subscriber,"Цена товара "+repository.GetItemById(itemId).ItemName +" изменилась и составила " + price, DateTime.Now);
        }
        command.Parameters.AddWithValue("@id", itemId);
        command.Parameters.AddWithValue("@price", price);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }
}