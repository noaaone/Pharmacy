using Npgsql;
using Pharmacy_.Models;

namespace Pharmacy_.Repositories;

public class UserRepository : RepositoryBase 
{
    public void WriteNewUserToDatabase(string login, string password)
    {
        AuthorizationRepository ar = new AuthorizationRepository();
        password = ar.Hash(password);
        string salt = ar.GetSalt();
        password = ar.Hash(password + salt);
        var user = new User(1, login, password, 0, salt, 1,false, false);
        using var connection = new NpgsqlConnection(connectionString);
       
        var sql = "INSERT INTO pharmacy.users (login, password, balance, salt, role, is_deleted, is_blocked) " +
                  "VALUES (@login, @password, @balance, @salt, @role, @is_deleted, @is_blocked)";

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@login", user.Login);
        command.Parameters.AddWithValue("@password", user.Password);
        command.Parameters.AddWithValue("@balance", user.Balance);
        command.Parameters.AddWithValue("@salt", user.Salt);
        command.Parameters.AddWithValue("@role", user.Role);
        command.Parameters.AddWithValue("@is_deleted", user.IsDeleted);
        command.Parameters.AddWithValue("@is_blocked", user.IsBlocked);

        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    public bool IsLoginUnique(string login)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "SELECT COUNT(*) FROM pharmacy.users WHERE login = @login";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@login", login);
        OpenConnection(connection);
        int count = Convert.ToInt32(command.ExecuteScalar());
        CloseConnection(connection);
        return Convert.ToBoolean(count);
    }
    
    public User Login(string login, string password)
    {
        AuthorizationRepository ar = new AuthorizationRepository();
        password = ar.Hash(password);
        string checkSalt = GetSaltByUserName(login);
        password = ar.Hash(password + checkSalt);
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        User user;
        connection.Open();
        var sql = "SELECT * FROM pharmacy.users WHERE login = @login AND password = @password";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("login", login);
            command.Parameters.AddWithValue("password", password);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    login = reader.GetString(1);
                    password = reader.GetString(2);
                    double balance = reader.GetDouble(3);
                    string salt = reader.GetString(4);
                    int role = reader.GetInt32(5);
                    Boolean isBlocked = reader.GetBoolean(6);
                    Boolean isDeleted = reader.GetBoolean(7);
                    user = new User(id, login, password,balance, salt, role, isBlocked, isDeleted);
                    connection.Close();
                    return user;
                }
            }
        }
        connection.Close();
        return null;
    }

    public string GetSaltByUserName(string name)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT salt FROM pharmacy.users WHERE login = @name";
        string salt = null;
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("name", name);
                
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                   salt = reader.GetString(0);
                }
            }
        }
        connection.Close(); // закрываем подключение
        return salt;
    }
    
    public User GetUserById(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        User user;
        connection.Open();
        var sql = "SELECT * FROM pharmacy.users WHERE id = @userId";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("userId", userId);
                
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
                    user = new User(id, login, password,balance, salt, role, isBlocked, isDeleted);
                    connection.Close();
                    return user;
                }
            }
        }
        connection.Close(); // закрываем подключение
        return null;
    }
    
    public void ChangeUserName(int id, string newLogin)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE pharmacy.users SET login = @newLogin WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@newLogin", newLogin);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    public void ChangePassword(int id, string password, string oldPassword)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE pharmacy.users SET password = @password WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@password", password);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        AddPasswordInHistoryList(id, oldPassword);
        CloseConnection(connection);
    }
    
    public void AddPasswordInHistoryList(int userId, string password)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "INSERT INTO pharmacy.recent_passwords  (password, user_id) VALUES (@password, @user_id)";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@password", password);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    
    public List<RecentPasswords> GetUserPasswordsHistory(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        List<RecentPasswords> passwords = new List<RecentPasswords>();
        connection.Open();
        var sql = "SELECT * FROM pharmacy.recent_passwords WHERE user_id = @user_id";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("user_id", userId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string password = reader.GetString(1);
                    int userIdRead = reader.GetInt32(2);
                    passwords.Add(new RecentPasswords(id, password, userIdRead));
                }
            }
        }
        connection.Close();
        return passwords;
    }

    public void ReplenishBalance(int id, double sumOfDep)
    {
        var ar = new AdminRepository();
        ar.ChangeUserBalance(id,GetUserById(id).Balance + sumOfDep);
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "INSERT INTO pharmacy.deposits (date, sum_of_deposit, user_id) VALUES (@date, @sum, @user_id)";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@date", DateTime.Now);
        command.Parameters.AddWithValue("@sum", sumOfDep);
        command.Parameters.AddWithValue("@user_id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
}
