using Npgsql;
using Pharmacy_.DTO;
using Pharmacy_.MiddleWares;
using Pharmacy_.Models;

namespace Pharmacy_.Repositories;

public class ItemsRepository : RepositoryBase
{
    public List<Item> GetItemListFromDataBase()
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT * FROM public.items";
        List<Item> items = new List<Item>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int manufacturerId = reader.GetInt32(2);
                    double price = reader.GetDouble(3);
                    string view = reader.GetString(4);
                    double priceOfView = reader.GetDouble(5);
                    int quantity = reader.GetInt32(6);
                    items.Add(new Item(id, name, manufacturerId, price, quantity, 
                        view, priceOfView));
                }
            }
        }
        connection.Close();
        return items;
    }

    public Item GetItemById(int itemId)
    {
        var connection = new NpgsqlConnection(connectionString);
        var sql = "SELECT * FROM public.items WHERE @id = id";
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
                    int quantity = reader.GetInt32(6);
                    connection.Close();
                    return new Item(id, name, manufacturerId, price, quantity, view, priceOfView);
                }
            }
        }
        connection.Close();
        return null;
    }
    
    public Item GetItemByItemName(string name)
    {
        var connection = new NpgsqlConnection(connectionString);
        var sql = "SELECT * FROM public.items WHERE @name = item_name";
        connection.Open();
        using (var command = new NpgsqlCommand(sql,connection))
        {
            command.Parameters.AddWithValue("@name",name);
            using(var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name1 = reader.GetString(1);
                    int manufacturerId = reader.GetInt32(2);
                    double price = reader.GetDouble(3);
                    string view = reader.GetString(4);
                    double priceOfView = reader.GetDouble(5);
                    int quantity = reader.GetInt32(6);
                    connection.Close();
                    return new Item(id, name1, manufacturerId, price, quantity, view, priceOfView);
                }
            }
        }
        connection.Close();
        return null;
    }
    public void DeleteItem(int id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE FROM public.items WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    
    public void WriteItemToDataBase(string itemName, int manufacturerId, double price, int quantity, string expertView, double priceOfExpert)
    {
        
        
        var item = new Item(1, itemName, manufacturerId,  price,quantity, expertView, priceOfExpert);
        var sql = "INSERT INTO public.items (item_name, manufacturer_id, price, quantity, expert_view, expert_view_price) " +
                  "VALUES (@item_name, @manufacturer_id, @price, @quantity, @expertView, @expertViewPrice)";

        using var connection = new NpgsqlConnection(connectionString);
        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@item_name", item.ItemName);
        command.Parameters.AddWithValue("@manufacturer_id", item.ManufacturerId);
        command.Parameters.AddWithValue("@price", item.Price);
        command.Parameters.AddWithValue("@quantity", item.Quantity);
        command.Parameters.AddWithValue("@expertView", item.ExpertView);
        command.Parameters.AddWithValue("@expertViewPrice", item.ExpertViewPrice);
            
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        
    }
    

    public void AddRecentPrice(Item item)
    {
        var connection = new NpgsqlConnection(connectionString);
        var sql = "INSERT INTO public.recent_prices (price, date, item_id)" +
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
        var sql = "SELECT * FROM public.recent_prices WHERE item_id = @id";
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
        var sql = "INSERT INTO public.purchases_of_view  (price, user_id, item_id) VALUES (@price, @user_id, @item_id)";
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
        var sql = "SELECT COUNT(*) FROM public.purchases_of_view WHERE user_id = @userId and item_id= @itemId";
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
        var sql = "SELECT expert_view FROM public.items WHERE @id = id";
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
        var sql = "SELECT * FROM public.items WHERE manufacturer_id = @manufacturerId";
        List<Item> items = new List<Item>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@manufacturerId",manufacturer_id);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int manufacturerId = reader.GetInt32(2);
                    double price = reader.GetDouble(3);
                    string view = reader.GetString(4);
                    double priceOfView = reader.GetDouble(5);
                    int quantity = reader.GetInt32(6);
                    items.Add(new Item(id, name, manufacturerId, price, quantity,view, priceOfView));
                }
            }
        }
        connection.Close();
        return items;
    }
    public void AddItemToBasket(AddItemToBasketRequest request)
    {
        var item = GetItemById(request.ItemId);
        if (request.Quantity > item.Quantity)
            throw new Exception("Неверное количество товара");
        var ar = new UserRepository();
        var user = ar.GetUserById(request.UserId);
        if (user == null)
            throw new Exception("Нет такого пользователя");
        
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "INSERT INTO public.basket  (quantity, price, user_id, item_id) " +
                  "VALUES (@quantity ,@price, @user_id, @item_id)";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@quantity", request.Quantity);
        command.Parameters.AddWithValue("@price", item.Price);
        command.Parameters.AddWithValue("@user_id", request.UserId);
        command.Parameters.AddWithValue("@item_id", request.ItemId);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        var item1 = GetItemById(request.ItemId);
        UpdateItem(request.ItemId, item1.Quantity - request.Quantity);
        }

    public Basket GetBasketItemById(int id)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql1 = "SELECT * FROM public.basket WHERE id = @id";
        using (var command1 = new NpgsqlCommand(sql1, connection))
        {
            command1.Parameters.AddWithValue("@id",id);
            using (var reader = command1.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id1 = reader.GetInt32(0);
                    int quantity = reader.GetInt32(1);
                    int user_id = reader.GetInt32(2);
                    int itemId = reader.GetInt32(3);
                    double price = reader.GetDouble(4);
                    
                    var basket = new Basket(id1, quantity,price, user_id, itemId);
                    CloseConnection(connection);
                    return basket;
                }
            }
        }
        CloseConnection(connection);
        return null;
    }
    
    public void DeleteItemFromBasket(int id)
    {
        var basketItem = GetBasketItemById(id);
        var item1 = GetItemById(basketItem.ItemId);
        UpdateItem(basketItem.ItemId, item1.Quantity+basketItem.Quantity);
        
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE FROM public.basket WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    public List<Basket> GetAllBasket(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT * FROM public.basket WHERE user_id = @userId";
        var items = new List<Basket>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@userId",userId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int quantity = reader.GetInt32(1);
                    int user_id = reader.GetInt32(2);
                    int itemId = reader.GetInt32(3);
                    double price = reader.GetDouble(4);
                    
                    items.Add(new Basket(id, quantity,itemId, user_id, itemId));
                }
            }
        }
        connection.Close();
        return items;
    }
    public List<Order> GetAllOrders()
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT * FROM public.order";
        var orders = new List<Order>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string orderInfo = reader.GetString(1);
                    int user_id = reader.GetInt32(2);
                    string status = reader.GetString(3);
                    DateTime date = reader.GetDateTime(4);
                    double fullPrice = reader.GetDouble(5);
                    
                    orders.Add(new Order(id, orderInfo,user_id, status, date, fullPrice));
                }
            }
        }
        connection.Close();
        return orders;
    }
    
    public List<Order> GetUserOrders(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = "SELECT * FROM public.order where user_id = @userId";
        
        var orders = new List<Order>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue(@"userId", userId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string orderInfo = reader.GetString(1);
                    int user_id = reader.GetInt32(2);
                    string status = reader.GetString(3);
                    DateTime date = reader.GetDateTime(4);
                    double fullPrice = reader.GetDouble(5);
                    
                    orders.Add(new Order(id, orderInfo,user_id, status, date, fullPrice));
                }
            }
        }
        connection.Close();
        return orders;
    }
    
    public void CreateOrder(int userId)
    {
        if (GetAllBasket(userId).Count == 0)
            throw new IncorrectDataException("В корзине нет товаров");
        string order = "";
        double fullPrice = 0;
        foreach (var b in GetAllBasket(userId))
        {
            var item = GetItemById(b.ItemId);
            order += $"{item.ItemName} в количестве {b.Quantity} по цене {b.Price}. \n";
            fullPrice += b.Price * b.Quantity;
        }
        WriteOrderToDatabase(userId, order, fullPrice);
       
        foreach (var b in GetAllBasket(userId))
        {
            DeleteBasketNoRefund(b.Id);
        }
        
    }

    public void DeleteBasketNoRefund(int id)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE FROM public.basket WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    public void ChangeQuantityOfItem(int itemId, double quantity)
    {
        const string connectionString = "Host=127.0.0.1;Port=5432;Database=kursach;Username=postgres;Password=postgres;";
        var connection = new NpgsqlConnection(connectionString);
        var sr = new SubjectRepository();
        ItemsRepository repository = new ItemsRepository();
        var sql = "UPDATE public.items SET quantity = @quantity WHERE id = @id";
        var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", itemId);
        command.Parameters.AddWithValue("@quantity", quantity);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }

    public void UpdateItem(int itemId, int quantity)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE public.items SET quantity = @quantity WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", itemId);
        command.Parameters.AddWithValue("@quantity", quantity);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }

    public void WriteOrderToDatabase(int userId, string order, double fullPrice)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "INSERT INTO public.order  (order_info, user_id, status, date, full_price) " +
                  "VALUES (@orderInfo ,@user_id, @status, @date, @fullPrice)";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@orderInfo", order);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@status", "принят");
        command.Parameters.AddWithValue("@date", DateTime.UtcNow);
        command.Parameters.AddWithValue("@fullPrice", fullPrice);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
}