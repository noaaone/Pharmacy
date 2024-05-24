using Microsoft.AspNetCore.Mvc;
using Pharmacy_.DTO;
using Pharmacy_.Models;
using Pharmacy_.Repositories;

namespace Pharmacy_.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpGet, Route("GetLogin")]
    public async Task<IActionResult> GetLogin(string login)
    {
        UserRepository userRepository = new UserRepository();
        _logger.LogInformation("Логин проверен");
        return Ok(userRepository.IsLoginUnique(login));
    }

    [HttpPut, Route("ChangeUserName")]
    public async Task<ActionResult> ChangeUserName([FromBody] ChangeLoginRequest request)
    {
        var ur = new UserRepository();
        try
        {
            if (ur.IsLoginUnique(request.NewLogin))
            {
                _logger.LogInformation("Этот логин уже используется");
                return BadRequest("Этот логин уже используется");
            }
            
            ur.ChangeUserName(request.Id, request.NewLogin);
            _logger.LogInformation("Логин успешно изменен");
            return Ok("Логин успешно изменен");
        }
        catch (Exception)
        {
            _logger.LogInformation("Логин не изменен");
            return BadRequest("Логин не изменен");
        }
    }

    [HttpPut, Route("ChangePassword")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var ur = new UserRepository();
        var user = ur.GetUserById(request.Id);
        try
        {
            AuthorizationRepository ar = new AuthorizationRepository();
            string oldPassword = request.Password;
            request.Password = ar.Hash(request.Password);
            request.Password = ar.Hash(request.Password + user.Salt);
            if (user.Password != request.Password)
            {
                return UnprocessableEntity("Пароли не совпадают");
            }

            if (request.NewPassword.Length == 0)
            {
                return UnprocessableEntity("Заполните поле");
            }

            if (request.NewPassword.Length > 32)
            {
                return UnprocessableEntity("Длина нового пароля должна быть меньше 32 символов");
            }

            if (request.NewPassword.Length < 4)
            {
                return UnprocessableEntity("Длина нового пароля должна быть больше 3 симаолов");
            }
            request.NewPassword = ar.Hash(request.NewPassword);
            request.NewPassword = ar.Hash(request.NewPassword + user.Salt);
            ur.ChangePassword(request.Id, request.NewPassword, oldPassword);
            return Ok("Пароль изменен");
        }
        catch (Exception)
        {
            return BadRequest("Пароль не изменен");
        }
    }

    [HttpPut, Route("ReplenishBalance")]
    public async Task<ActionResult> ReplenishBalance([FromBody] ReplenishBalanceRequest request)
    {
        try
        {
            int sum = Convert.ToInt32(request.SumOfDep);
            if (sum < 0)
            {
                return UnprocessableEntity("Сумма пополнения должна быть положительной");
            }

            var ur = new UserRepository();
            ur.ReplenishBalance(request.Id, sum);
            _logger.LogInformation("Пополнение суммой  = " + request.SumOfDep + " успешно");
            return Ok("Пополнение суммой  = " + request.SumOfDep + " успешно");
        }
        catch (Exception e)
        {
            _logger.LogInformation("Депозит отменен");
            return BadRequest("Депозит отменен");
        }
    }

    [HttpPost, Route("AddCreditCard")]
    public async Task<ActionResult> AddCreditCard([FromBody] AddCreditCardRequest request)
    {
        try
        {
            var cr = new CreditCardRepository();
            if (!cr.IsValidCreditCardNumber(request.CardNumber))
            {
                _logger.LogInformation("Неверный номер карты");
                return UnprocessableEntity("Неверный номер карты");
            }
            
            if (!cr.IsValidCardholderName(request.Name))
            {
                _logger.LogInformation("Неверное имя держателя");
                return UnprocessableEntity("Неверное имя держателя");
            }

            if (!cr.IsValidExpirationDate(request.GoodTrue))
            {
                _logger.LogInformation("Неверное время действия");
                return UnprocessableEntity("Неверное время действия");
            }

            if (!cr.IsValidCVV(request.Cvv))
            {
                _logger.LogInformation("Неверный cvv");
                return UnprocessableEntity("Неверный cvv");
            }
            request.CardNumber = cr.EditNumber(request.CardNumber);
            foreach (var cardNumber in cr.GetNumberOfCardsOfUser(request.Id))
            {
                if (request.CardNumber.Equals(cardNumber))
                {
                    _logger.LogInformation("Вы уже добавили эту карту");
                    return UnprocessableEntity("Вы уже добавили эту карту");
                }
            }

            cr.WriteNewCreditCardToDatabase(request.Id, request.CardNumber, request.Name, request.GoodTrue,
                Convert.ToInt32(request.Cvv));
            _logger.LogInformation("Карта добавлена");
            return Ok("Карта добавлена");
        }
        catch (Exception e)
        {
            _logger.LogInformation("Невозможно добавить карту");
            return BadRequest("Невозможно добавить карту");
        }
    }

    [HttpGet, Route("GetItemList")]
    public async Task<ActionResult> GetItemList()
    {
        try
        {
            ItemsRepository itemsRepository = new ItemsRepository();
            _logger.LogInformation("Список товаров доступен");
            List<Item> items = itemsRepository.GetItemListFromDataBase();
            return Ok(items);
        }
        catch (Exception)
        {
            _logger.LogInformation("Список товаров недоступен");
            return BadRequest("Список товаров недоступен");
        }
    }

    [HttpDelete, Route("DeleteCreditCard")]
    public async Task<ActionResult> DeleteCreditCard(int id)
    {
        try
        {
            var cr = new CreditCardRepository();
            cr.DeleteCreditCard(id);
            _logger.LogInformation("Карта с id = " + id + " удалена");
            return Ok("Карта с id = " + id + " удалена");
        }
        catch (Exception e)
        {
            _logger.LogInformation("Карта с id = " + id + "не удалена");
            return Ok("Карта с id = " + id + "не удалена");
        }
    }

    [HttpPost, Route("BuyExpertView")]
    public async Task<ActionResult> BuyExpertView([FromBody] ExpertViewRequest request)
    {
        try
        {
        var sr = new ItemsRepository();
        var ur = new UserRepository();
        if (ur.GetUserById(request.UserId).Balance < sr.GetItemById(request.ItemId).ExpertViewPrice)
        {
            _logger.LogInformation("На балансе недостаточно средств");
            return UnprocessableEntity("На балансе недостаточно средств");
        }

        if (sr.IsAlreadyBought(request.UserId, request.ItemId))
        {
            _logger.LogInformation("Вы уже купили это мнение");
            return UnprocessableEntity("Вы уже купили это мнение");
        }

        sr.AddPurchace(request.UserId, request.ItemId, sr.GetItemById(request.ItemId).ExpertViewPrice);
        _logger.LogInformation("Мнение куплено");
        return Ok("Мнение куплено");
        }
        catch (Exception e)
        {
            _logger.LogInformation("Невозможно купить мнение");
            return BadRequest("Невозможно купить мнение");
        }
    }

    [HttpGet, Route("GetExpertView")]
    public async Task<ActionResult> GetExpertView(int userId, int itemId)
    {
        try
        {
            var sr = new ItemsRepository();
            if (!sr.IsAlreadyBought(userId, itemId))
            {
                _logger.LogInformation("Мнение не куплено");
                return Ok("Мнение не куплено");
            }

            return Ok(sr.GetExpertView(itemId));
        }
        catch (Exception e)
        {
            _logger.LogInformation("Невозможно получить мнение");
            return BadRequest("Невозможно получить мнение");
        }
    }

    [HttpPost, Route("AddSubscription")]
    public async Task<ActionResult> AddSubscription([FromBody] SubscriptionRequest request)
    {
        try
        {
            var sr = new SubjectRepository();
            foreach (var subscriber in sr.GetSubscribesList(request.ItemId))
            {
                if (subscriber == request.UserId)
                {
                    _logger.LogInformation("Вы уже подписаны");
                    return UnprocessableEntity("Вы уже подписаны");
                }
            }
            sr.Subscribe(request.UserId, request.ItemId);
            _logger.LogInformation("Подписка успешна");
            return Ok("Подписка успешна");
        }
        catch (Exception e)
        {
            _logger.LogInformation("Невозможно подписаться");
            return BadRequest("Невозможно подписаться");
        }
    }
    
    [HttpDelete, Route("DeleteSubscription")]
    public async Task<ActionResult> DeleteSubscription([FromBody] SubscriptionRequest request)
    {
        try
        {
            var sr = new SubjectRepository();
            sr.Unsubscribe(request.UserId, request.ItemId);
            _logger.LogInformation("Отписка успешна");
            return Ok("Отписка успешна");
        }
        catch (Exception e)
        {
            _logger.LogInformation("Невозможно отписаться");
            return BadRequest("Невозможно отписаться");
        }
    }

    [HttpDelete, Route("DeleteNotification")]
    public async Task<ActionResult> DeleteNotification(int id)
    {
        try
        {
            var sr = new SubjectRepository();
            sr.DeleteNotificationById(id);
            _logger.LogInformation("Уведоление удалено");
            return Ok("Уведоление удалено");
        }
        catch (Exception e)
        {
            _logger.LogInformation("Уведоление не удалено");
            return BadRequest("Уведоление не удалено");
        }
    }

    [HttpGet, Route("GetNotificationList")]
    public async Task<ActionResult> GetNotificationList(int userId)
    {
        try
        {
            var sr = new SubjectRepository();
            var notifications = sr.GetNotificationsList(userId);
            if (notifications.Count == 0)
            {
                _logger.LogInformation("Лист уведомлений пуст");
                return Ok("Лист уведомлений пуст");
            }
            _logger.LogInformation("Лист уведомлений доступен");
            return Ok(notifications);
        }
        catch (Exception e)
        {
            _logger.LogInformation(" Лист уведомлений недоступен");
            return Ok(" Лист уведомлений недоступен");
        }
    }
    
    [HttpGet, Route("GetItemListByManufacturerId")]
    public async Task<ActionResult> GetItemListByManufacturerId(int id)
    {
        try
        {
            ItemsRepository itemsRepository = new ItemsRepository();
            _logger.LogInformation("Товары производителя доступны");
            List<Item>  items = itemsRepository.GetItemListByManufacturerId(id);
            return Ok(items);
        }
        catch (Exception)
        {
            _logger.LogInformation("Товары производителя недоступны");
            return BadRequest("Товары производителя недоступны");
        }
    }

    [HttpGet, Route("GetSCountOfCards")]
    public async Task<ActionResult> GetSCountOfCards(int id)
    {
        try
        {
            CreditCardRepository cr = new CreditCardRepository();
            
            bool count = cr.HasCards(id);
            _logger.LogInformation("Число карт доступно");
            return Ok(count);
        }
        catch (Exception)
        {
            _logger.LogInformation("Невозможно вернуть число карт");
            return BadRequest("Невозможно вернуть число карт");
        }
    }

    [HttpGet, Route("GetNumberOfNotifications")]
    public async Task<ActionResult> GetNumberOfNotifications(int id)
    {
        try
        {
            SubjectRepository sr = new SubjectRepository();
            int number = sr.GetNumberOfNotifications(id);
            _logger.LogInformation("Количество уведомлений равно " + number);
            return Ok(number);
        }
        catch (Exception e)
        {
            _logger.LogInformation("Посчитать количество уведомлений не удалось");
            return BadRequest("Посчитать количество уведомлений не удалось");
        }
    }
}