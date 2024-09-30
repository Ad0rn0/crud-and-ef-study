using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.Extensions;

public static class ModelStateExtension
{
    public static List<string> GetErrors(this ModelStateDictionary modelState)
    {
        var errorsList = new List<string>();
        foreach (var errors in modelState.Values)
        {
            foreach (var error in errors.Errors)
            {
                errorsList.Add(error.ErrorMessage);
            }
        }
        return errorsList;
    }
}