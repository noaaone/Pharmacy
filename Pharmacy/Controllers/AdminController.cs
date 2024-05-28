using Microsoft.AspNetCore.Mvc;
using Pharmacy_.DTO;
using Pharmacy_.Models;
using Pharmacy_.Repositories;

namespace Pharmacy_.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

        [HttpGet, Route("GetUserById")]
        public async Task<IActionResult> GetUserById(int userId)
    {
        UserRepository userRepository = new UserRepository();
        try
        {
            User user = userRepository.GetUserById(userId);
            Console.WriteLine(user.toString());
            _logger.LogInformation("Пользователь получен");
            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogInformation("Пользователь не найден");
            return BadRequest("Пользователь не найден");
        }
    }

        
        [HttpPut, Route("SetStatusDel")]
        public async Task<ActionResult> SetStatusDel([FromBody] AddSetStatusDeletedRequest request)
        {
            try
            {
                var ur = new AdminRepository();
                ur.SetUserStatusDel(request.Id, request.IsDeleted);
                _logger.LogInformation("Статус изменен");
                return Ok("Статус изменен");
            }
            catch (Exception)
            {
                _logger.LogInformation("Статус не изменен");
                return BadRequest("Статус не изменен");
            }
        }

        [HttpDelete, Route("DeleteAccount")]
        public async Task<ActionResult> DeleteAccount(int id)
    {
        try
        {
            var ur = new AdminRepository();
            ur.DeleteUser(id);
            _logger.LogInformation("Аккаунт успешно удален");
            return Ok("Аккаунт успешно удален");
        }
        catch (Exception)
        {
            _logger.LogInformation("Аккаунт не удален");
            return BadRequest("Аккаунт не удален");
        }
    }

        [HttpPut, Route("SetStatusBlock")]
        public async Task<ActionResult> SetStatusBlock([FromBody] AddSetStatusBlockedRequest request)
        {
            try
            {
                var ur = new AdminRepository();
                ur.SetUserStatusBlock(request.Id, request.IsBlocked);
                _logger.LogInformation("Статус изменен");
                return Ok("Статус изменен");
            }
            catch (Exception)
            {
                _logger.LogInformation("Статус не изменен");
                return BadRequest("Статус не изменен");
            }
        }

        [HttpGet, Route("GetUserList")]
        public async Task<ActionResult> GetUserList()
    {
        var ur = new AdminRepository();
        try
        {
            _logger.LogInformation("Список пользователей доступен");
            return Ok(ur.GetUserList());
        }
        catch (Exception)
        {
            _logger.LogInformation("Список пользователей недоступен");
            return BadRequest("Список пользователей недоступен");
        }
    }

        [HttpPut, Route("ChangeRole")]
        public async Task<ActionResult> ChangeRole([FromBody] AddSetRoleRequest request)
        {
            try
            {
                var ar = new AdminRepository();
                ar.ChangeUserRole(request.Id, request.Role);
                _logger.LogInformation("Роль изменена");
                return Ok("Роль изменена");
            }
            catch (Exception)
            {
                _logger.LogInformation("Роль не изменена");
                return BadRequest("Роль не изменена");
            }
        }

        [HttpGet, Route("GetUserPreviousPasswordsList")]
        public async Task<ActionResult> GetUserPreviousPasswordsList(int id)
        {
            var ur = new UserRepository();
            try
            {
                var passwords = ur.GetUserPasswordsHistory(id);
                if (passwords.Count == 0)
                {
                    _logger.LogInformation("Здесь нет паролей");
                    return Ok("Здесь нет паролей");
                }
                _logger.LogInformation("Пароли доступны");
                return Ok(passwords);
            }
            catch (Exception)
            {
                return BadRequest("Невозможно получить пароли");
            }
        }

        [HttpPost("AddManufacturer")]
        public async Task<ActionResult> AddManufacturer([FromBody] AddManufacturerRequest request)
        {
            try
            {
                var ur = new AdminRepository();
                var ar = new AuthorizationRepository();
                if (!ar.IsValidEmail(request.Email))
                {
                    _logger.LogInformation("Email неверный");
                    return UnprocessableEntity("Email неверный");
                }

                ur.WriteNewManufacturerToDatabase(request.Name, request.Country, request.Phone, request.Email);
                _logger.LogInformation("Докто добавлен");
                return Ok("Производитель добавлен");
            }
            catch (Exception e)
            {
                _logger.LogInformation("Производитель не добавлен");
                return BadRequest("Производитель не добавлен");
            }
        }

        [HttpDelete, Route("DeleteManufacturer")]
        public async Task<ActionResult> DeleteManufacturer(int id)
        {
            try
            {
                var ar = new AdminRepository();
                ar.DeleteManufacturer(id);
                _logger.LogInformation("Производитель удален");
                return Ok("Производитель удален");
            }
            catch (Exception e)
            {
                _logger.LogInformation("Производитель не удален");
                return BadRequest("Производитель не удален");
            }
        }

        [HttpGet, Route("GetManufacturerList")]
        public async Task<ActionResult> GetManufacturerList()
        {
            var ur = new AdminRepository();
            try
            {
                _logger.LogInformation("Список производителей доступен");
                return Ok(ur.GetManufacturerList());
            }
            catch (Exception)
            {
                _logger.LogInformation("Список производителей недоступен");
                return BadRequest("Список производителей недоступен");
            }
        }

        [HttpPost("AddItem")]
        public async Task<ActionResult> AddItem([FromBody] AddItemRequest request)
        {
            try
            {
                if (request.Price < 0)
                {
                    _logger.LogInformation("Цена должна быть положительной");
                    return UnprocessableEntity("Цена должна быть положительной");
                }
                if (request.Quantity < 0)
                {
                    _logger.LogInformation("Количество должно быть положительным");
                    return UnprocessableEntity("Количество должно быть положительным");
                }
                var sr = new ItemsRepository();
                if (sr.GetItemByItemName(request.ItemName)!=null)
                {
                    _logger.LogInformation("Товар с таким именем уже существует");
                    return UnprocessableEntity("Товар с таким именем уже существует");
                }
                var ar = new AdminRepository();
                if (ar.IsThereThisManufacturer(request.ManufacturerId) == false)
                {
                    _logger.LogInformation("Производитель с id = " + request.ManufacturerId + " не существует");
                    return BadRequest("Производитель с id = " + request.ManufacturerId + " не существует");
                }

                if (request.ExpertViewPrice < 0)
                {
                    _logger.LogInformation("Цена экспертного мнения должна быть положительной");
                    return UnprocessableEntity("Цена экспертного мнения должна быть положительной");
                }
                
                sr.WriteItemToDataBase(request.ItemName, request.ManufacturerId, request.Price,request.Quantity, request.ExpertView, request.ExpertViewPrice);
                
                sr.AddRecentPrice(sr.GetItemByItemName(request.ItemName));
                   
                
                _logger.LogInformation("Товар добавлен");
                return Ok("Товар добавлен");
            }
            catch (Exception e)
            {
                _logger.LogInformation("Товар не добавлен");
                return BadRequest("Товар не добавлен");
            }
        }

        [HttpPut, Route("ChangePriceOfItem")]
        public async Task<ActionResult> ChangePriceOfItem([FromBody] AddChangePriceRequest request)
        {
             try
             {
                if (request.Price < 0)
                {
                    return UnprocessableEntity("Цена должна быть положительной");
                }
                var sr = new ItemsRepository();
                var or = new ObserverRepository();
                if (request.Price == sr.GetItemById(request.Id).Price)
                {
                    _logger.LogInformation("Цена должна отличаться");
                    return UnprocessableEntity("Цена должна отличаться");
                }
                or.ChangePrice(request.Id, request.Price);
                sr.AddRecentPrice(sr.GetItemById(request.Id));
                _logger.LogInformation("Цена товара с id = " + request.Id + " равна " + request.Price);
                return Ok("Цена товара с id = " + request.Id + " равна " + request.Price);
            }
            catch (Exception e)
            {
             _logger.LogInformation("Цена не изменена");
                return BadRequest("Цена не изменена");
            }
        }
    
        [HttpPut, Route("ChangeQuantityOfItem")]
        public async Task<ActionResult> ChangeQuantityOfItem([FromBody] ChangeQuantityRequest request)
        {
            try
            {
                if (request.Quantity < 0)
                {
                    return UnprocessableEntity("Количество должно быть положительным");
                }
                var sr = new ItemsRepository();
                var or = new ObserverRepository();
                if (request.Quantity == sr.GetItemById(request.Id).Quantity)
                {
                    _logger.LogInformation("Количество должно отличаться");
                    return UnprocessableEntity("Количество должно отличаться");
                }
                sr.ChangeQuantityOfItem(request.Id, request.Quantity);
                _logger.LogInformation("Количество товара с id = " + request.Id + " равно " + request.Quantity);
                return Ok("Количество товара с id = " + request.Id + " равно " + request.Quantity);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Количество не изменено");
                return BadRequest("Количество не изменено");
            }
        }

        [HttpGet, Route("GetRecentPricesList")]
        public async Task<ActionResult> GetRecentPricesList(int id)
        {
            try
            {
                var sr = new ItemsRepository();
                if (sr.GetRecentPricesListFromDataBase(id).Count == 0)
                {
                    _logger.LogInformation("Тут нет цен");
                    return Ok("Тут нет цен");
                }
                _logger.LogInformation("Лист цен доступен");
                return Ok(sr.GetRecentPricesListFromDataBase(id));
            }
            catch (Exception e)
            {
                _logger.LogInformation("Лист цен недоступен");
                return BadRequest("Лист цен недоступен");
            }
        }

        [HttpGet, Route(("GetLoginHistoryList"))]
        public async Task<ActionResult> GetLoginHistoryList(int id)
        {
            try
            {
                var ar = new AdminRepository();
                var loginList = ar.GetLoginHistory(id);
                _logger.LogInformation("История входов доступна");
                return Ok(loginList);
            }
            catch (Exception e)
            {
                _logger.LogInformation("История входов недоступна");
                return BadRequest("История входов недоступна");
            }
        }
        
        [HttpDelete, Route("DeleteItem")]
        public async Task<ActionResult> DeleteItem(int id)
        {
            try
            {
                var ur = new ItemsRepository();
                ur.DeleteItem(id);
                _logger.LogInformation("Товар с id = " + id + " удален");
                return Ok("Товар с id = " + id + " удален");
            }
            catch (Exception)
            {
                _logger.LogInformation("Товар с id = " + id + "не удален");
                return BadRequest("Товар с id = " + id + "не удален");
            }
        }

        [HttpPut, Route("EditExpertView")]
        public async Task<ActionResult> EditExpertView([FromBody] EditExpertViewRequest request)
        {
            try
            {
                var ar = new AdminRepository();
                ar.EditExpertView(request.ItemId, request.NewView, request.NewPriceOfView);
                _logger.LogInformation("Мнение изменено");
                return Ok("Мнение изменено");
            }
            catch (Exception e)
            {
                _logger.LogInformation("Невозможно изменить мнение");
                return BadRequest("Невозможно изменить мнение");
            }
        }
        
        [HttpGet, Route(("GetManufacturerById"))]
        public async Task<ActionResult> GetManufacturerById(int id)
        {
            try
            {
                var ar = new AdminRepository();
                var manufacturer = ar.GetManufacturerByID(id);
                if (!ar.IsThereThisManufacturer(id))
                {
                    return BadRequest("Здесь нет этого производителя");
                }
                _logger.LogInformation("Производитель с id = " + id + " доступен");
                return Ok(manufacturer);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Производитель с id = " + id + " недоступен");
                return BadRequest("Производитель с id = " + id + " недоступен");
            }
        }

        [HttpGet, Route(("GetManufacturerNameById"))]
        public async Task<ActionResult> GetManufacturerNameById(int id)
        {
            try
            {
                var ar = new AdminRepository();
                var name = ar.GetManufacturerNameById(id);
                if (!ar.IsThereThisManufacturer(id))
                {
                    return BadRequest("Здесь нет этого производителя");
                }
                _logger.LogInformation("Имя производителя с id = " + id + " доступно");
                return Ok(name);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Имя производителя с id = " + id + " недоступно");
                return BadRequest("Имя производителя с id = " + id + " недоступно");
            }
        }

        [HttpGet, Route("GetDepositsOfUser")]
        public async Task<ActionResult> GetDepositsOfUser(int userId)
        {
            try
            {
                var ar = new AdminRepository();
                 var deposits = ar.GetDepositsOfUser(userId);
                _logger.LogInformation("Депозиты доступны");
                return Ok(deposits);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Депозиты недоступны");
                return BadRequest("Депозиты недоступны");
            }
            
        }
        
        [HttpGet, Route("GetPurchasesOfUser")]
        public async Task<ActionResult> GetPurchasesOfUser(int userId)
        {
            try
            {
                var ar = new AdminRepository();
                var purchases = ar.GetPurchasesOfUser(userId);
                _logger.LogInformation("История покупок доступна");
                return Ok(purchases);
            }
            catch (Exception e)
            {
                _logger.LogInformation("История покупок недоступна");
                return BadRequest("История покупок недоступна");
            }
            
        }

        [HttpGet, Route("GetAllOrders")]
        public async Task<ActionResult> GetAllOrders()
        {
            var ar = new ItemsRepository();
            _logger.LogInformation("Список заказов получен");
            return Ok(ar.GetAllOrders());
        }
        
        [HttpGet, Route("GetUserOrders")]
        public async Task<ActionResult> GetUserOrders(int userId)
        {
            var ar = new ItemsRepository();
            _logger.LogInformation("Список заказов получен");
            return Ok(ar.GetUserOrders(userId));
        }
        
        [HttpPut, Route("EditOrderStatus")]
        public async Task<ActionResult> EditOrderStatus([FromBody] EditOrderStatusRequest request)
        {
            var ar = new AdminRepository();
                ar.EditOrderStatus(request);
                _logger.LogInformation("Статус изменен");
                return Ok("Статус изменен");
        }
        [HttpDelete, Route("DeleteOrder")]
        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            var ar = new AdminRepository();
            ar.DeleteOrder(orderId);
            _logger.LogInformation("Заказ удален");
            return Ok("Заказ удален");
        }
}