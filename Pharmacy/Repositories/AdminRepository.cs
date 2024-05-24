using Npgsql;
using Pharmacy_.DTO;
using Pharmacy_.Models;

namespace Pharmacy_.Repositories;

public class AdminRepository : RepositoryBase
{
    public void SetUserStatusDel(int id, bool isDeleted)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE pharmacy.users SET is_deleted = @isDeleted WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@isDeleted", isDeleted);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
        
    public void SetUserStatusBlock(int id, bool isBlocked)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE pharmacy.users SET is_blocked = @isBlocked WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@isBlocked", isBlocked);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
        
    public void DeleteUser(int id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE FROM pharmacy.users WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    
    public List<User> GetUserList()
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        User user;
        connection.Open();
        var sql = "SELECT * FROM pharmacy.users";
        List<User> users = new List<User>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string login = reader.GetString(1);
                    string password = reader.GetString(2);
                    double balance = reader.GetDouble(3);
                    string salt = reader.GetString(4);
                    int role = reader.GetInt32(5);
                    Boolean isBlocked = reader.GetBoolean(6);
                    Boolean isDeleted = reader.GetBoolean(7);
                    users.Add(new User(id, login, password,balance, salt, role, isBlocked, isDeleted));
                }
            }
        }
        connection.Close();
        return users;
    }
    
    public List<Manufacturer> GetManufacturerList()
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT * FROM pharmacy.manufacturers";
        List<Manufacturer> manufacturers = new List<Manufacturer>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string country = reader.GetString(2);
                    string phone = reader.GetString(3);
                    string email = reader.GetString(4);
                    manufacturers.Add(new Manufacturer(id, name, country,phone, email));
                }
            }
        }
        connection.Close();
        return manufacturers;
    }
    
    /*public void EditUser(int id,User user)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE pharmacy.users SET (login, password, balance, salt, role, is_deleted, is_blocked) " +
                  "= (@login, @password, @balance, @salt, @role, @is_deleted, @is_blocked) WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@login", user.Login);
        command.Parameters.AddWithValue("@password", user.Password);
        command.Parameters.AddWithValue("@balance", user.Balance);
        command.Parameters.AddWithValue("@salt", user.Salt);
        command.Parameters.AddWithValue("@role", user.Role);
        command.Parameters.AddWithValue("@is_deleted", user.IsDeleted);
        command.Parameters.AddWithValue("@is_blocked", user.IsBlocked);
        command.Parameters.AddWithValue("@id", user.Id);
        
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }*/

    public void WriteNewManufacturerToDatabase(string name, string country, string phone, string email)
    {
        var manufacturer = new Manufacturer(1, name, country, phone, email);
        var sql = "INSERT INTO pharmacy.manufacturers (name, country, phone, email) " +
                  "VALUES (@name, @country, @phone, @email)";

        using var connection = new NpgsqlConnection(connectionString);
        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@name", manufacturer.Name);
        command.Parameters.AddWithValue("@country", manufacturer.Country);
        command.Parameters.AddWithValue("@phone", manufacturer.Phone);
        command.Parameters.AddWithValue("@email", manufacturer.Email);

        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    
    public void DeleteManufacturer(int id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE FROM pharmacy.manufacturers WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    
   
    
    public bool IsThereThisManufacturer(int manufacturerId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        var sql = "SELECT COUNT(*) FROM pharmacy.manufacturers WHERE id = @manufacturerId";
        var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("manufacturerId", manufacturerId);
        OpenConnection(connection);
        int count = Convert.ToInt32(command.ExecuteScalar());
        CloseConnection(connection);
        return Convert.ToBoolean(count);
    }
    
    public void ChangeUserRole(int id, int role)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE pharmacy.users SET role = @role WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@role", role);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    public void ChangeUserBalance(int id, double balance)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE pharmacy.users SET balance = @balance WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@balance", balance);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    public List<LoginHistory> GetLoginHistory(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        List<LoginHistory> loginHistories = new List<LoginHistory>();
        var sql = "SELECT * FROM pharmacy.login_history WHERE user_id = @userID ";
        OpenConnection(connection);
        using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string ip = reader.GetString(1);
                    DateTime date = reader.GetDateTime(2);
                    int user_id = reader.GetInt32(3);
                    loginHistories.Add(new LoginHistory(id ,ip, date, user_id));
                }
            }
        }
        CloseConnection(connection);
        return loginHistories;
    }

    public void EditExpertView(int id, string view, double price)
    {
        DeletePurchases(id);
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE pharmacy.items SET (expert_view,expert_view_price) " +
                  "= (@view,@price) WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@view", view);
        command.Parameters.AddWithValue("@price", price);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }

    public void DeletePurchases(int itemId)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE FROM pharmacy.purchases_of_view WHERE item_id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", itemId);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }

    public Manufacturer GetManufacturerByID(int manufacturerId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        Manufacturer manufacturer;
        connection.Open();
        var sql = "SELECT * FROM pharmacy.manufacturers WHERE id = @manufacturerId";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("manufacturerId", manufacturerId);
                
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string country = reader.GetString(2);
                    string phone = reader.GetString(3);
                    string email = reader.GetString(4);
                    manufacturer = new Manufacturer(id, name, country,phone, email);
                    connection.Close();
                    return manufacturer;
                }
            }
        }
        connection.Close(); // закрываем подключение
        return null;
    }

    public string GetManufacturerNameById(int id)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT name FROM pharmacy.manufacturers WHERE id = @manufacturerId";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("manufacturerId", id);
                
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    connection.Close();
                    return name;
                }
            }
        }
        connection.Close(); // закрываем подключение
        return null;
    }

    public List<Deposit> GetDepositsOfUser(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        List<Deposit> deposits = new List<Deposit>();
        connection.Open();
        var sql = "SELECT * FROM pharmacy.deposits WHERE user_id = @userId";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("userId", userId);
                
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    double sum = reader.GetDouble(1);
                    DateTime date = reader.GetDateTime(2);
                    int userID = reader.GetInt32(3);
                    deposits.Add(new Deposit(id, sum, date, userID));
                }
            }
        }
        connection.Close(); // закрываем подключение
        return deposits;
        
    }
    
    public List<Purchase>GetPurchasesOfUser(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        List<Purchase> purchases = new List<Purchase>();
        connection.Open();
        var sql = "SELECT * FROM pharmacy.purchases_of_view WHERE user_id = @userId";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("userId", userId);
                
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    double price = reader.GetDouble(1);
                    int userID = reader.GetInt32(2);
                    int itemID = reader.GetInt32(3);
                    purchases.Add(new Purchase(id, price, userID, itemID));
                }
            }
        }
        connection.Close(); // закрываем подключение
        return purchases;
        
    }

    public void EditOrderStatus(EditOrderStatusRequest request)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE pharmacy.order SET status " +
                  "= @status WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", request.OrderId);
        command.Parameters.AddWithValue("@status", request.Status);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    public void DeleteOrder(int orderId)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "Delete from pharmacy.order WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", orderId);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
}