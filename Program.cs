
using Microsoft.AspNetCore.Builder;
using Application.Models;
using Application.Services;


var builder = WebApplication.CreateBuilder(args);

//DI registration
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();

//Swagger
builder.Services.AddEndpointsApiExplorer(); 
//builder.Services.AddSwaggerGen();

var app = builder.Build();


//Lets build our Minimal CRUD api endpoints
app.MapGet("/", () => "Hello \n Welcome to the User management Application!");

//To get all users
app.MapGet("/users", (IUserRepository repo) => {
    if(repo.GetAllUsers().Count() == 0)
    {
        return Results.NotFound("No users found.");
    }
    return Results.Ok(repo.GetAllUsers());
});

app.MapGet("/users/{id:int:min(1)}", (int id, IUserRepository repo) => {
    var user = repo.GetUserById(id);
    if(user == null)
    {
        return Results.NotFound($"User with id {id} not found.");
    }
    return Results.Ok(user);
});

app.MapPost("/users",async (User newUser, IUserRepository repo) => {
    var addedUser = repo.AddUser(newUser);
    return Results.Created($"/users/{addedUser.Id}", addedUser);
});

app.MapPut("/users/{id:int:min(1)}", (int id, User updatedUser, IUserRepository repo) => {
    var existingUser = repo.GetUserById(id);
    if(existingUser == null)
    {
        return Results.NotFound($"User with id {id} not found.");
    }
    repo.UpdateUser(id, updatedUser with { Id = id });
    return Results.Ok(updatedUser with { Id = id });
});

app.MapDelete("/users/{id:int:min(1)}", (int id, IUserRepository repo) => {
    var existingUser = repo.GetUserById(id);
    if(existingUser == null)
    {
        return Results.NotFound($"User with id {id} not found.");
    }
    repo.DeleteUser(id);
    return Results.Ok($"User with id {id} deleted successfully.");
});

app.Run();
