using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using asp_net_ecommerce_web_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_ecommerce_web_api.Controllers
{
    [ApiController]
    [Route("api/categories/")]
    public class CategoryController : ControllerBase
    {


        private static List<Category> categories = new List<Category>();

        // GET: /api/categories => Read categories
        [HttpGet]
        public IActionResult GetCategories([FromQuery] string searchValue = "")
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                var searchedCategories = categories.Where(c => c.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();
                return Ok(searchedCategories);
            }

            return Ok(categories);
        }

        // POST: /api/categories => Create a category
        [HttpPost]
        public IActionResult CreateCategory([FromBody] Category categoryData)
        {
            // Console.WriteLine($"{categoryData}");

            if (string.IsNullOrEmpty(categoryData.Name))
            {
                return BadRequest("Category Name is required and can not be empty");
            }
            if (categoryData.Name.Length < 2)
            {
                return BadRequest("Category name must be atleast 2 characters long");
            }

            var newCategory = new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = categoryData.Name,
                Description = categoryData.Description,
                CreatedAt = DateTime.UtcNow,
            };
            categories.Add(newCategory);
            return Created($"/api/categories/{newCategory.CategoryId}", newCategory);
        }

        // PUT: /api/categories/{categoryId} => Update a category
        [HttpPut("{categoryId:guid}")]
        public IActionResult UpdateCategoryById(Guid categoryId, [FromBody] Category categoryData)
        {
            if (categoryData == null)
            {
                return BadRequest("Category data is missing");
            }

            var foundCategory = categories.FirstOrDefault(category => category.CategoryId == categoryId);
            if (foundCategory == null)
            {
                return NotFound("Category with this id does not exist");
            }

            if (!string.IsNullOrEmpty(categoryData.Name))
            {
                if (categoryData.Name.Length >= 2)
                {
                    foundCategory.Name = categoryData.Name;
                }
                else
                {
                    return BadRequest("Category name must be atleast 2 characters long");
                }
            }

            if (!string.IsNullOrWhiteSpace(categoryData.Description))
            {
                foundCategory.Description = categoryData.Description;
            }

            return NoContent();
        }

        // DELETE: /api/categories/{categoryId} => Delete a category by Id
        [HttpDelete("{categoryId:guid}")]
        public IActionResult DeleteCategoryById(Guid categoryId)
        {
            var foundCategory = categories.FirstOrDefault(category => category.CategoryId == categoryId);
            if (foundCategory == null)
            {
                return NotFound("Category with this id does not exist");
            }
            categories.Remove(foundCategory);
            return NoContent();
        }
    }
}