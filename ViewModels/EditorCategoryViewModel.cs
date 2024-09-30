using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels;

public class EditorCategoryViewModel
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(40,
        MinimumLength = 3,
        ErrorMessage = "Category name must be between 3 and 40 characters")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Category slug is required")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Category slug must be between 3 and 40 characters")]
    public string Slug { get; set; }
}