using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<Category> categories = new List<Category>();

app.MapGet("/", () => "Api is working fine");

// GET : /api/categories => Read  Categories
app.MapGet("/api/categories", ([FromQuery] string searchValue = "") =>
{

  if (!string.IsNullOrEmpty(searchValue))
  {
    var searchedCategories = categories.Where(c => c.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();
    return Results.Ok(searchedCategories);
  }

  return Results.Ok(categories);
});

// POST : /api/categories => Create a Category
app.MapPost("/api/categories", ([FromBody] Category categoryData) =>
{
  // Console.WriteLine($"{categoryData}");

  if (string.IsNullOrEmpty(categoryData.Name))
  {
    return Results.BadRequest("Category Name is required and can not be empty");
  }
  if (categoryData.Name.Length > 2)
  {
    return Results.BadRequest("Category name must be atleast 2 characters long");
  }

  var newCategory = new Category
  {
    CategoryId = Guid.NewGuid(),
    Name = categoryData.Name,
    Description = categoryData.Description,
    CreatedAt = DateTime.UtcNow,
  };
  categories.Add(newCategory);
  return Results.Created($"/api/categories/{newCategory.CategoryId}", newCategory);

});

// DELETE : /api/categories/{categoryId} => Delete a Category
app.MapDelete("/api/categories/{categoryId:guid}", (Guid categoryId) =>
{
  var foundCategory = categories.FirstOrDefault(category => category.CategoryId == categoryId);
  if (foundCategory == null)
  {
    return Results.NotFound("Category with this id does not exist");
  }
  categories.Remove(foundCategory);
  return Results.NoContent();
});

// PUT : /api/categories/{categoryId} => Update a Category
app.MapPut("/api/categories/{categoryId:guid}", (Guid categoryId, [FromBody] Category categoryData) =>
{
  if (categoryData == null)
  {
    return Results.BadRequest("Category data is missing");
  }

  var foundCategory = categories.FirstOrDefault(category => category.CategoryId == categoryId);
  if (foundCategory == null)
  {
    return Results.NotFound("Category with this id does not exist");
  }

  if (!string.IsNullOrEmpty(categoryData.Name))
  {
    if (categoryData.Name.Length >= 2)
    {
      foundCategory.Name = categoryData.Name;
    }
    else
    {
      return Results.BadRequest("Category name must be atleast 2 characters long");
    }
  }

  if (!string.IsNullOrWhiteSpace(categoryData.Description))
  {
    foundCategory.Description = categoryData.Description;
  }

  return Results.NoContent();
});


app.Run();

public record Category
{
  public Guid CategoryId { get; set; }
  public string Name { get; set; }
  public string Description { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }

};

// CRUD 
// Create => Create a Category => POST : /api/categories 
// Read => Read a Category => GET : /api/categories 
// Update => Update a Category => PUT : /api/categories 
// Delete => Deelete a Category => Delete : /api/categories 