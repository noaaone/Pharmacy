using Microsoft.AspNetCore.Mvc;
using Pharmacy_.DTO;
using Pharmacy_.Repositories;

namespace Pharmacy_.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(ILogger<AuthorizationController> logger)
    {
        _logger = logger;
    }
    
    [HttpPost, Route("Register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (request.Password.Length < 4)
            {
                _logger.LogInformation("Длина пароля должна быть больше 4 символов");
                return UnprocessableEntity("Длина пароля должна быть больше 4 символов");
            }

            if (request.Password != request.PasswordRepeat)
            {
                _logger.LogInformation("Пароли не совпадают");
                return UnprocessableEntity("Пароли не совпадают");
            }

            var ur = new UserRepository();
            if (ur.IsLoginUnique(request.Login))
            {
                _logger.LogInformation("Такой логин уже используется");
                return UnprocessableEntity("Такой логин уже используется");
            }
            
            ur.WriteNewUserToDatabase(request.Login, request.Password);
            _logger.LogInformation("Аккаунт успешно создан");
            return Ok("Аккаунт успешно создан");
        }
        catch (Exception)
        {
            _logger.LogInformation("Аккаунт не создан");
            return BadRequest("Аккаунт не создан");
        }
    }

    [HttpPost, Route("Login")]
    public async Task<ActionResult> Login([FromBody] AuthorizationRequest request)
    {
        try
        {
            var ur = new UserRepository();
            var ar = new AuthorizationRepository();

            var user = ur.Login(request.Login, request.Password);
            if (user == null)
            {
                _logger.LogInformation("Неправильный логин или пароль");
                return UnprocessableEntity("Неправильный логин или пароль");
            }

            if (user.IsBlocked)
            {
                _logger.LogInformation("Вы заблокированы");
                return UnprocessableEntity("Вы заблокированы");
            }

            if (user.IsDeleted)
            {
                _logger.LogInformation("Ваш аккаунт удален");
                return UnprocessableEntity("Ваш аккаунт удален");
            }

            if (user != null)
            {
                ar.SaveAccountLoginHistory(user.Id);
                return Ok(user);
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation("Ошибка в авторизации");
            return UnprocessableEntity("Ошибка в авторизации");
        }
        return null;
    }
}