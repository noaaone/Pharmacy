using Npgsql;

namespace Pharmacy_.Repositories;

public class RepositoryBase
{
    protected const string connectionString = "Host=localhost;Port=5432;Database=Pharmacy;Username=postgres;Password=postgres;";

    protected void OpenConnection(NpgsqlConnection connection)
    {
        connection.Open();
    }
        
    protected void CloseConnection(NpgsqlConnection connection)
    {
        connection.Close();
    }
}