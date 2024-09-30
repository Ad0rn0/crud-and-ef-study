using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

// [Authorize]
[ApiController]
public class AccountController : ControllerBase
{
    // private readonly TokenService _tokenService; // Injeção de Dependência
    //
    // public AccountController(TokenService tokenService)
    // {
    //     _tokenService = tokenService;
    // }
    // [AllowAnonymous]
    [HttpPost("v1/login")]
    public IActionResult Login(
        [FromServices] TokenService tokenService)
    {
        var token = tokenService.GenerateToken(null);
        
        return Ok(token);
    }

    [Authorize(Roles = "user")]
    [HttpGet("v1/user")]
    public IActionResult GetUser()
    {
        return Ok(User.Identity.Name);
    }
    
    [Authorize(Roles = "author")]
    [HttpGet("v1/author")]
    public IActionResult GetAuthor()
        => Ok(User.Identity.Name);
    
    [Authorize(Roles = "admin")]
    [HttpGet("v1/admin")]
    public IActionResult GetAdmin()
        => Ok(User.Identity.Name);
}