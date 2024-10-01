using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

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

    [HttpPost("v1/accounts/")]
    public async Task<IActionResult> PostAsync(
        [FromBody] RegisterViewModel model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User()
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace('@', '-').Replace('.', '-')
            };

            var password = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(password);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email, password
            }));
        }
        catch (DbUpdateException dbException)
        {
            return StatusCode(400, new ResultViewModel<string>(dbException.Message));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>($"Internal Server Error."));
        }

        return null;
    }
    
    [HttpPost("v1/accounts/login")]
    public IActionResult Login(
        [FromServices] TokenService tokenService)
    {
        var token = tokenService.GenerateToken(null);

        return Ok(token);
    }
}