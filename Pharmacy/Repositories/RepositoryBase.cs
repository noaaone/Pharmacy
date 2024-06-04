using Npgsql;

namespace Pharmacy_.Repositories;

public class RepositoryBase
{
    protected const string connectionString = "Host=127.0.0.1;Port=5432;Database=kursach;Username=postgres;Password=postgres;";


    protected void OpenConnection(NpgsqlConnection connection)
    {
        connection.Open();
    }
        
    protected void CloseConnection(NpgsqlConnection connection)
    {
        connection.Close();
    }
}