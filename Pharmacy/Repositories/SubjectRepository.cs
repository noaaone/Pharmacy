using Npgsql;
using Pharmacy_.Interfaces;
using Pharmacy_.Models;

namespace Pharmacy_.Repositories;

public class SubjectRepository : ISubject 
{
    const string connectionString = "Host=127.0.0.1;Port=5432;Database=kursach;Username=postgres;Password=postgres;";
    
    public void Subscribe(int userId, int itemId)
    {
        using var connection = new NpgsqlConnection(connectionString);
       
        var sql = "INSERT INTO public.subscriptions (user_id, item_id) " +
                  "VALUES (@userId, @itemId)";

        using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@itemId", itemId);
        
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }

    public void Unsubscribe(int userId, int itemId)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE FROM public.subscriptions WHERE user_id = @id and item_id = @itemId";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", userId);
        command.Parameters.AddWithValue("@itemId", itemId);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }

    public void Notify(int userId,string notification,DateTime date)
    {
        using var connection = new NpgsqlConnection(connectionString);
       
        var sql = "INSERT INTO public.notifications (text, user_id,date) " +
                  "VALUES (@notification, @userId,@date)";

        using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.AddWithValue("@notification", notification);
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@date", date);
        
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }

    public List<int> GetSubscribesList(int itemId)
    {
        List<int> subscribers = new List<int>();
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT user_id FROM public.subscriptions WHERE item_id = @item_id";
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@item_id", itemId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        subscribers.Add(id);
                    }
                }
            }
        }
        return subscribers;
    }



    public void DeleteNotificationById(int id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE FROM public.notifications WHERE id = @id ";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }
    public List<Notification> GetNotificationsList(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        List<Notification> notifications = new List<Notification>();
        connection.Open();
        var sql = "SELECT * FROM public.notifications WHERE user_id = @user_id";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("user_id", userId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string text = reader.GetString(1);
                    DateTime date = reader.GetDateTime(3);
                    int userID = reader.GetInt32(2);
                    
                    
                    notifications.Add(new Notification(id, text, date, userId));
                }
            }
        }
        connection.Close();
        return notifications;
    }

    public int GetNumberOfNotifications(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT COUNT(*) FROM public.notifications WHERE user_id = @user_id";
        int count;
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("user_id", userId);
            count = Convert.ToInt32(command.ExecuteScalar());
            
        }
        connection.Close();
        return count;
    }
}