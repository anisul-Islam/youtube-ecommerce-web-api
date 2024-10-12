using Microsoft.AspNetCore.Mvc;

using asp_net_ecommerce_web_api.Controllers;

var builder = WebApplication.CreateBuilder(args);


// Add services to the controller 
builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
  options.InvalidModelStateResponseFactory = context =>
  {
    var errors = context.ModelState
                    .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                    .SelectMany(e => e.Value?.Errors != null ? e.Value.Errors.Select(x => x.ErrorMessage) : new List<string>()).ToList();
    return new BadRequestObjectResult(ApiResponse<object>.ErrorResponse(errors, 400, "Validation failed"));
  };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/", () => "Api is working fine");

app.MapControllers();
app.Run();



// CRUD 
// Create => Create a Category => POST : /api/categories 
// Read => Read a Category => GET : /api/categories 
// Update => Update a Category => PUT : /api/categories 
// Delete => Deelete a Category => Delete : /api/categories 

// MVC = Models, Views, Controllers