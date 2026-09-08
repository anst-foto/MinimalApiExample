using System;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MinimalApiExample.BusinessLogic;
using MinimalApiExample.DataAccess;


// ----- Настройка приложения -----
var builder = WebApplication.CreateBuilder(args);

// Регистрация зависимостей
builder.Services.AddSingleton<IRepository<Person>, PersonRepository>();
builder.Services.AddScoped<IService<Person>, PersonService>();

// Настройка сериализации JSON (camelCase, индентация)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

// Добавьте CORS-политику
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// ----- Глобальная обработка ошибок (middleware) -----
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            error = app.Environment.IsDevelopment() ? ex.Message : "An internal server error occurred.",
            stackTrace = app.Environment.IsDevelopment() ? ex.StackTrace : null
        };

        await context.Response.WriteAsJsonAsync(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
});

// ----- Эндпоинты -----

// GET /persons — получить всех
app.MapGet("/persons", async (IService<Person> service) =>
{
    try
    {
        var persons = service.GetAll();
        return Results.Ok(persons);
    }
    catch (Exception)
    {
        // Логирование можно добавить через ILogger
        return Results.Problem("Failed to retrieve persons", statusCode: 500);
    }
});

// GET /persons/{id} — получить по ID
app.MapGet("/persons/{id:guid}", async (Guid id, IService<Person> service) =>
{
    try
    {
        var person = service.FindById(id);
        return person is not null ? Results.Ok(person) : Results.NotFound();
    }
    catch (Exception)
    {
        return Results.Problem("Failed to retrieve person", statusCode: 500);
    }
});

// GET /persons/search?name=... — поиск по имени (частичное совпадение)
app.MapGet("/persons/search", async (string name, IService<Person> service) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(name))
            return Results.BadRequest("Name parameter cannot be empty");

        var result = service.FindByName(name);
        return Results.Ok(result);
    }
    catch (Exception)
    {
        return Results.Problem("Failed to search persons", statusCode: 500);
    }
});

// POST /persons — создать нового человека
app.MapPost("/persons", async (CreatePersonRequest request, IService<Person> service) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.FirstName))
        {
            return Results.BadRequest("LastName and FirstName are required");
        }

        var person = new Person(
            Guid.NewGuid(),
            request.LastName.Trim(),
            request.FirstName.Trim(),
            request.Patronymic?.Trim(),
            request.DateOfBirth
        );

        var success = service.Add(person);
        return success
            ? Results.Created($"/persons/{person.Id}", person)
            : Results.BadRequest("Failed to add person (repository error)");
    }
    catch (Exception)
    {
        return Results.Problem("Failed to create person", statusCode: 500);
    }
});

// PUT /persons/{id} — обновить существующего
app.MapPut("/persons/{id:guid}", async (Guid id, UpdatePersonRequest request, IService<Person> service) =>
{
    try
    {
        if (id != request.Id)
            return Results.BadRequest("ID in route does not match ID in request body");

        if (string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.FirstName))
        {
            return Results.BadRequest("LastName and FirstName are required");
        }

        var person = new Person(
            request.Id,
            request.LastName.Trim(),
            request.FirstName.Trim(),
            request.Patronymic?.Trim(),
            request.DateOfBirth
        );

        var success = service.Update(person);
        return success ? Results.Ok(person) : Results.NotFound();
    }
    catch (Exception)
    {
        return Results.Problem("Failed to update person", statusCode: 500);
    }
});

// DELETE /persons/{id} — удалить по ID
app.MapDelete("/persons/{id:guid}", async (Guid id, IService<Person> service) =>
{
    try
    {
        var existing = service.FindById(id);
        if (existing is null)
            return Results.NotFound();

        var success = service.Delete(existing);
        return success ? Results.NoContent() : Results.NotFound();
    }
    catch (Exception)
    {
        return Results.Problem("Failed to delete person", statusCode: 500);
    }
});

app.Run();

// ----- DTO для запросов -----
internal record CreatePersonRequest(
    string LastName,
    string FirstName,
    string? Patronymic,
    DateTime DateOfBirth
);

internal record UpdatePersonRequest(
    Guid Id,
    string LastName,
    string FirstName,
    string? Patronymic,
    DateTime DateOfBirth
);