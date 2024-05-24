using Microsoft.AspNetCore.Mvc;
using Pharmacy_.Repositories;

namespace Pharmacy_.Controllers;
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet, Route("GetItemById")]
    public async Task<ActionResult> GetItemById(int id)
    {
        try
        {
            var sr = new ItemsRepository();
            _logger.LogInformation("Получен товар с именем =" + sr.GetItemById(id).ItemName);
            return Ok(sr.GetItemById(id));
        }
        catch (Exception e)
        {
            return BadRequest("Товар не найден");
        }
    }

    [HttpGet, Route("GetUserCards")]
    public async Task<ActionResult> GetUserCards(int id)
    {
        var cr = new CreditCardRepository();
        return Ok(cr.GetCardsOfUser(id));
    }

    [HttpGet, Route("IsSubscribed")]
    public async Task<ActionResult> IsSubscribed(int userId,int itemId)
    {
        try
        {
            var sr = new SubjectRepository();
            foreach (var a in sr.GetSubscribesList(itemId))
            {
                if (userId == a)
                {
                    _logger.LogInformation("Пользователь подписан");
                    return Ok("Пользователь подписан");
                }
            }

            return Ok("Пользователь не подписан");
        }
        catch (Exception e)
        {
            _logger.LogInformation("Невозможно проверить подписку");
            return BadRequest("Невозможно проверить подписку");
        }
    }
}