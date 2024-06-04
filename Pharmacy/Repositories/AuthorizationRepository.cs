using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql;

namespace Pharmacy_.Repositories;

public class AuthorizationRepository: RepositoryBase
{
    public string GetSalt()
    {
        byte[] salt = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }

    public string Hash(string inputString)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
    
    public bool VerifyPassword(string password, string salt, string hashedPassword)
    {
        password = Hash(password);
        password = Hash(password + salt);
        return password == hashedPassword;
    }
    
    public bool IsValidEmail(string email)
    {
        var pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        if (Regex.Match(email, pattern).Success)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void SaveAccountLoginHistory(int id)
    {
        try
        {
            string? ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            var sql = "INSERT INTO public.login_history (ip, date, user_id) " +
                      "VALUES (@ip, @date, @user_id)";
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using var command = new NpgsqlCommand(sql, connection);
            if (ipAddress != null)
            {
                command.Parameters.AddWithValue("@ip", ipAddress);
                command.Parameters.AddWithValue("@date", DateTime.Now);
                command.Parameters.AddWithValue("@user_id", id);
                OpenConnection(connection);
                command.ExecuteNonQuery();
                CloseConnection(connection);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error. Can't update login history");
        }
    }
    
}