
using Microsoft.AspNetCore.Builder;
using Application.Models;
using Application.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Collections.Generic; 
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

//DI registration for InMemoryUserRepository
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();

//register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Application.Validators.UserValidator>();
builder.Services.AddFluentValidationAutoValidation();

//Register Swagger
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

//Register Http Logging Middleware
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;

    //Lets increase the body size limits for logging
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;

    // Ensure JSON is treated as text so bodies are logged 
    logging.MediaTypeOptions.AddText("application/json");
});

var app = builder.Build();

//Middleware for swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Middleware for Http Logging
app.UseHttpLogging();
//Custom Middleware to log request and response details
app.Use(async (context, next) =>
{
    // Custom Middleware Logic Before Next Middleware
    Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke(); // Call the next middleware
    // Custom Middleware Logic After Next Middleware
    Console.WriteLine($"Outgoing Response: {context.Response.StatusCode}");
}); 


//Lets build our Minimal CRUD api endpoints
app.MapGet("/", () => "Hello \n Welcome to the User management Application!");

//To get all users
app.MapGet("/users", (IUserRepository repo) => {
    var users = repo.GetAllUsers().ToList(); 
    return Results.Ok(users);
});

app.MapGet("/users/{id:int:min(1)}", (int id, IUserRepository repo) => {
    var user = repo.GetUserById(id);
    if(user == null)
    {
        return Results.NotFound($"User with id {id} not found.");
    }
    return Results.Ok(user);
});

app.MapPost("/users",async (User newUser, IUserRepository repo, IValidator<User> validator) => {
    var validation = await validator.ValidateAsync(newUser); 
    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

    var toAdd = newUser with { Id = 0 };
    var addedUser = repo.AddUser(toAdd);
    return Results.Created($"/users/{addedUser.Id}", addedUser);
});

app.MapPut("/users/{id:int:min(1)}", async (int id, User updatedUser, IUserRepository repo, IValidator<User> validator) => {
    var existingUser = repo.GetUserById(id);
    if(existingUser == null)
    {
        return Results.NotFound($"User with id {id} not found.");
    }
    var validation = await validator.ValidateAsync(updatedUser); 
    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

    var toUpdate = updatedUser with { Id = id, 
    CreatedAt = existingUser.CreatedAt, 
    IsActive = existingUser.IsActive, 
    UpdatedAt = DateTime.UtcNow }; 
    var ok = repo.UpdateUser(id, toUpdate);
    return Results.Ok(toUpdate);
});

app.MapDelete("/users/{id:int:min(1)}", (int id, IUserRepository repo) => {
    var deleted = repo.DeleteUser(id); 
    return deleted ? Results.NoContent() : Results.NotFound($"User with id {id} not found.");
});


app.Run();
