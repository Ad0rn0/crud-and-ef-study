using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts;

public class RegisterViewModel
{
    [Required(ErrorMessage = "You must enter a name")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "You must enter a email")]
    [EmailAddress(ErrorMessage = "You must enter a valid email")]
    public string Email { get; set; }
}