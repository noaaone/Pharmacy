using System.Text;
using System.Text.RegularExpressions;
using Npgsql;
using Pharmacy_.Models;

namespace Pharmacy_.Repositories;

public class CreditCardRepository : RepositoryBase
{
    public bool IsValidCreditCardNumber(string creditCardNumber)
    {
        // Удаляем пробелы из номера карты и проверяем, что длина номера от 13 до 19 символов
        creditCardNumber = creditCardNumber.Replace(" ", "");
        if (creditCardNumber.Length < 13 || creditCardNumber.Length > 19)
        {
            return false;
        }

        // Проверяем, что номер карты состоит только из цифр
        long number;
        if (!long.TryParse(creditCardNumber, out number))
        {
            return false;
        }
        // Вставляем пробел после каждой четвертой цифры
        
        // Проверяем, что номер карты проходит проверку Луна
        int sum = 0;
        bool alternate = false;
        for (int i = creditCardNumber.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(creditCardNumber[i].ToString());
            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }
            sum += digit;
            alternate = !alternate;
        }
        return (sum % 10 == 0);
    }
    
    public bool IsValidCardholderName(string name)
    {
        // Проверяем, что имя держателя состоит только из букв и пробелов
        if (!Regex.IsMatch(name, @"^[a-zA-Z ]+$"))
        {
            return false;
        }

        // Проверяем, что длина имени держателя не превышает 26 символов
        if (name.Length > 26)
        {
            return false;
        }

        return true;
    }
    
    public bool IsValidExpirationDate(string date)
    {
        // Проверяем, что дата действия состоит из 4 цифр и разделителя "/"
        if (!Regex.IsMatch(date, @"^\d{2}/\d{2}$"))
        {
            return false;
        }

        // Разбиваем дату на месяц и год
        string[] parts = date.Split('/');
        int month = int.Parse(parts[0]);
        int year = int.Parse(parts[1]);

        // Проверяем, что месяц и год находятся в допустимых пределах
        if (month < 1 || month > 12 || year < DateTime.Now.Year % 100)
        {
            return false;
        }

        return true;
    }
    
    public bool IsValidCVV(string cvv)
    {
        // Проверяем, что CVV состоит из 3 или 4 цифр
        if (!Regex.IsMatch(cvv, @"^\d{3,4}$"))
        {
            return false;
        }

        return true;
    }

    public string EditNumber(string cardNumber)
    {
        StringBuilder modifiedNumber = new StringBuilder();
        for (int i = 0; i < cardNumber.Length; i++)
        {
            modifiedNumber.Append(cardNumber[i]);
            if ((i + 1) % 4 == 0 && (i + 1) < cardNumber.Length)
            {
                modifiedNumber.Append(" ");
            }
        }
        cardNumber = modifiedNumber.ToString();
        return cardNumber;
    }
    
    public void WriteNewCreditCardToDatabase(int userId,string cardNumber, string name, string date,int cvv)
    {
        var sql = "INSERT INTO pharmacy.cards (card_number, name, date, cvv, user_id) " +
                  "VALUES (@cardNumber, @name, @date, @cvv, @userId)";
        using var connection = new NpgsqlConnection(connectionString);
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@cardNumber", cardNumber);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@date", date);
        command.Parameters.AddWithValue("@cvv", cvv);
        command.Parameters.AddWithValue("@userId", userId);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    
    public void DeleteCreditCard(int id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE FROM pharmacy.cards WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }

    public List<string> GetNumberOfCardsOfUser(int id)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        var sql = "SELECT card_number FROM pharmacy.cards WHERE user_id = @id";
        var cardsList = new List<string>();
        OpenConnection(connection);
        using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", id);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    cardsList.Add(reader.GetString(0));
                }
            }
        }
        CloseConnection(connection);
        return cardsList;
    }

    public List<Card> GetCardsOfUser(int id)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        var sql = "SELECT * FROM pharmacy.cards WHERE user_id = @id";
        var cardsList = new List<Card>();
        OpenConnection(connection);
        using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", id);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int cardId = reader.GetInt32(0);
                    string cardNumber = reader.GetString(1);
                    string name = reader.GetString(2);
                    string date = reader.GetString(3);
                    int cvv = reader.GetInt32(4);
                    int userId = reader.GetInt32(5);
                    cardsList.Add(new Card(cardId, cardNumber, name, date, cvv, userId));
                }
            }
        }
        CloseConnection(connection);
        return cardsList;
    }
    
    public bool HasCards(int id)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            var sql = "SELECT COUNT(*) FROM pharmacy.cards WHERE user_id = @userId";
            var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("userId", id);
            OpenConnection(connection);
            int count = Convert.ToInt32(command.ExecuteScalar());
            CloseConnection(connection);
            return count != 0;
        }
    }
}