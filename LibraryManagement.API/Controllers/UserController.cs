using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Domain.UserEntity;
using LibraryManagement.Application.Services;



namespace LibraryManagement.API.Controllers;


[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _userService.GetUsers();
        return Ok(users);
    }


    [HttpPost]
    public IActionResult CreateUser(User user)
    {
        var result = _userService.CreateUser(user);
        return Ok(result);
    }
}