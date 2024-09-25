using Blog.Data;
using Blog.Models;
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
                => Ok(await context.Categories.ToListAsync());

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            
            if (category == null)
                return NotFound();
            
            return Ok(category);
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] Category category,
            [FromServices] BlogDataContext context)
        {
            try
            {
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
            [FromBody] Category categoryBody,
            [FromServices] BlogDataContext context)
        {
            var categoryToUpdate = await context
                .Categories
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (categoryToUpdate == null)
                return NotFound();
            
            categoryToUpdate.Name = categoryBody.Name;
            categoryToUpdate.Slug = categoryBody.Slug;
             
            context.Categories.Update(categoryToUpdate);
            await context.SaveChangesAsync();
            
            return Ok(categoryToUpdate);
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
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
    }
}
