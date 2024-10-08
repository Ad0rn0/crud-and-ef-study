using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Blog.Controllers;

public class EmailController : Controller
{
    [HttpPost("v1/email")]
    public IActionResult SendEmail(
        [FromBody] JsonElement data, // Mudei para JsonElement
        [FromServices] EmailServices emailService)
    {
        string toName = data.GetProperty("toName").GetString();
        string toEmail = data.GetProperty("toEmail").GetString();
        string subject = data.GetProperty("subject").GetString();
        string body = data.GetProperty("body").GetString();

        if (toEmail == null)
            return NotFound(data);

        if (emailService.Send(toName, toEmail, subject, body))
            return Ok("Sended");
        return BadRequest();
    }
}