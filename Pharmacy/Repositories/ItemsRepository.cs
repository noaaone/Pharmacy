using Npgsql;
using Pharmacy_.Models;

namespace Pharmacy_.Repositories;

public class ItemsRepository : RepositoryBase
{
    public List<Item> GetItemListFromDataBase()
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT * FROM pharmacy.items";
        List<Item> items = new List<Item>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string itemName = reader.GetString(1);
                    int manufacturerId = reader.GetInt32(2);
                    double price = reader.GetDouble(3);
                    string view = reader.GetString(4);
                    double priceOfView = reader.GetDouble(5);
                    items.Add(new Item(id, itemName, manufacturerId, price, view, priceOfView));
                }
            }
        }
        connection.Close();
        return items;
    }

    public Item GetItemById(int itemId)
    {
        var connection = new NpgsqlConnection(connectionString);
        var sql = "SELECT * FROM pharmacy.items WHERE @id = id";
        connection.Open();
        using (var command = new NpgsqlCommand(sql,connection))
        {
            command.Parameters.AddWithValue("@id",itemId);
            using(var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int manufacturerId = reader.GetInt32(2);
                    double price = reader.GetDouble(3);
                    string view = reader.GetString(4);
                    double priceOfView = reader.GetDouble(5);
                    connection.Close();
                    return new Item(id, name, manufacturerId, price, view, priceOfView);
                }
            }
        }
        connection.Close();
        return null;
    }
    
    public void DeleteItem(int id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE FROM pharmacy.items WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    
    public void WriteItemToDataBase(string itemName, int manufacturerId, double price, string expertView, double priceOfExpert)
    {
        
        
        var item = new Item(1, itemName, manufacturerId,  price, expertView, priceOfExpert);
        var sql = "INSERT INTO pharmacy.items (item_name, manufacturer_id, price, expert_view, expert_view_price) " +
                  "VALUES (@item_name, @manufacturer_id, @price, @expertView, @expertViewPrice)";

        using var connection = new NpgsqlConnection(connectionString);
        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@item_name", item.ItemName);
        command.Parameters.AddWithValue("@manufacturer_id", item.ManufacturerId);
        command.Parameters.AddWithValue("@price", item.Price);
        command.Parameters.AddWithValue("@expertView", item.ExpertView);
        command.Parameters.AddWithValue("@expertViewPrice", item.ExpertViewPrice);
            
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        
    }
    

    public void AddRecentPrice(Item item)
    {
        var connection = new NpgsqlConnection(connectionString);
        var sql = "INSERT INTO pharmacy.recent_prices (price, date, item_id)" +
                  " VALUES (@price, @date, @itemId)";
        var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@price", item.Price);
        command.Parameters.AddWithValue("@date", DateTime.Now);
        command.Parameters.AddWithValue("@itemId", item.Id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    
    public List<RecentPrices> GetRecentPricesListFromDataBase(int recentPriceId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT * FROM pharmacy.recent_prices WHERE item_id = @id";
        List<RecentPrices> prices = new List<RecentPrices>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", recentPriceId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    double price = reader.GetDouble(1);
                    DateTime date = reader.GetDateTime(2);
                    int itemId = reader.GetInt32(3);
                    prices.Add(new RecentPrices(id, price, date, itemId));
                }
            }
        }
        connection.Close();
        return prices;
    }

    public void AddPurchace(int userId, int itemId, double priceOfView)
    {
        var ar = new AdminRepository();
        var ur = new UserRepository();
        ar.ChangeUserBalance(userId,ur.GetUserById(userId).Balance - priceOfView);
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "INSERT INTO pharmacy.purchases_of_view  (price, user_id, item_id) VALUES (@price, @user_id, @item_id)";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@price", priceOfView);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@item_id", itemId);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }

    public bool IsAlreadyBought(int userId, int itemId)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "SELECT COUNT(*) FROM pharmacy.purchases_of_view WHERE user_id = @userId and item_id= @itemId";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@itemId", itemId);
        OpenConnection(connection);
        int count = Convert.ToInt32(command.ExecuteScalar());
        CloseConnection(connection);
        return Convert.ToBoolean(count);
    }

    public string GetExpertView(int itemId)
    {
        var connection = new NpgsqlConnection(connectionString);
        var sql = "SELECT expert_view FROM pharmacy.items WHERE @id = id";
        connection.Open();
        using (var command = new NpgsqlCommand(sql,connection))
        {
            command.Parameters.AddWithValue("@id",itemId);
            using(var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string view = reader.GetString(0);
                    connection.Close();
                    return view;
                }
            }
        }
        connection.Close();
        return null;
    }
    
    public List<Item> GetItemListByManufacturerId(int manufacturer_id)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT * FROM pharmacy.items WHERE manufacturer_id = @manufacturerId";
        List<Item> items = new List<Item>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@manufacturerId",manufacturer_id);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string itemName = reader.GetString(1);
                    int manufacturerId = reader.GetInt32(2);
                    double price = reader.GetDouble(3);
                    string view = reader.GetString(4);
                    double priceOfView = reader.GetDouble(5);
                    items.Add(new Item(id, itemName, manufacturerId, price, view, priceOfView));
                }
            }
        }
        connection.Close();
        return items;
    }
    
}