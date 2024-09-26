using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context)
        {
            try
            {
                var categories = await context.Categories.ToListAsync();
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
                    return NotFound();

                return Ok(category);
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
                return BadRequest();
            
            try
            {
                var category = new Category
                {
                    Name = categoryViewModel.Name,
                    Slug = categoryViewModel.Slug.ToLower()
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", category);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Unable to save category");
            }
            catch
            {
                return StatusCode(500, "Internal server error");
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
                    return NotFound();

                var category = new Category()
                {
                    Name = categoryViewModel.Name,
                    Slug = categoryViewModel.Slug.ToLower()
                };

                categoryToUpdate.Name = category.Name;
                categoryToUpdate.Slug = category.Slug;

                context.Categories.Update(categoryToUpdate);
                await context.SaveChangesAsync();
            
                return Ok(categoryToUpdate);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Unable to update category");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
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
                    return NotFound();

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(category);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Unable to delete category");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
