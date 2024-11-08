using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync(
            [FromServices] IMemoryCache cache,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var categories = cache.GetOrCreate("CategoriesCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return context.Categories.ToList();
                });
                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Database Update Error"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Internal server errror"));
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return StatusCode(404, new ResultViewModel<List<Category>>("Category not found"));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Database Update Error"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Internal server error"));
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryViewModel categoryViewModel,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return StatusCode(400, new ResultViewModel<List<Category>>(ModelState.GetErrors()));
            
            try
            {
                var category = new Category
                {
                    Name = categoryViewModel.Name,
                    Slug = categoryViewModel.Slug.ToLower()
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
                
                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Database Update Error"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Internal server error"));
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel categoryViewModel,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var categoryToUpdate = await context
                    .Categories
                    .FirstOrDefaultAsync(c => c.Id == id);
            
                if (categoryToUpdate == null)
                    return StatusCode(404, new ResultViewModel<List<Category>>("Category not found"));

                var category = new Category()
                {
                    Name = categoryViewModel.Name,
                    Slug = categoryViewModel.Slug.ToLower()
                };

                categoryToUpdate.Name = category.Name;
                categoryToUpdate.Slug = category.Slug;

                context.Categories.Update(categoryToUpdate);
                await context.SaveChangesAsync();
            
                return Ok(new ResultViewModel<Category>(categoryToUpdate));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Database Update Error"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Internal server error"));
            }
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context
                    .Categories
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return StatusCode(404, new ResultViewModel<List<Category>>("Category not found"));

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Database Update Error"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Internal server error"));
            }
        }
    }
}
