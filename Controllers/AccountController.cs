using System.Text.RegularExpressions;
using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
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
        [FromServices] EmailServices emailService,
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
            
            if (await context.Users.AnyAsync(x => x.Email == model.Email))
                return BadRequest(new ResultViewModel<string>($"Email {model.Email} already exists"));

            var password = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(password);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            string templatePath = "Template/WelcomeEmail.html";
            string htmlBody = System.IO.File.ReadAllText(templatePath);
            
            htmlBody = htmlBody.Replace("{{UserName}}", user.Name)
                .Replace("{{UserPassword}}", password);
            
            emailService.Send(user.Name,
                user.Email,
                "Bem vindo ao nosso Blog!",
                htmlBody
            );
            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email,
                password
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
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] TokenService tokenService,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        
        var user = await context
            .Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
            return StatusCode(401, new ResultViewModel<string>($"User or password is incorrect. "));
        
        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<string>("User or password is incorrect."));

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(data: token));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>($"Internal Server Error."));
        }
    }

    [Authorize]
    [HttpPost("v1/accounts/upload-image")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageViewModel model,
        [FromServices] BlogDataContext context)
    {
        var fileName = $"{Guid.NewGuid().ToString()}.jpg";
        var data = new Regex(@"^data:image\/[a-z]+;base64,")
            .Replace(model.Base64Image, string.Empty);
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResultViewModel<string>("Internal Server Error."));
        }
        
        var user = await context
            .Users
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        
        if (user == null)
            return NotFound(new ResultViewModel<string>($"User not found."));
        
        user.Image = $"https://localhost:7155/images/{fileName}";

        try
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResultViewModel<string>("Internal Server Error."));
        }
        
        return Ok(new ResultViewModel<string>(data: "Image successfully uploaded."));
        
    }
}