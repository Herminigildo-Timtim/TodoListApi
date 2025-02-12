using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.Models;

namespace TodoListApi.Endpoints;

public static class TodoEndpoints
{
    public static RouteGroupBuilder MapTodoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/todo");

        group.MapPost("", async (TodoDbContext db, TodoItem todo) =>
        {
            var newTodo = new TodoItem
            {
                Title = todo.Title,
                IsCompleted = todo.IsCompleted,
                DateCreated = DateTime.UtcNow
            };
            db.TodoItems.Add(newTodo);
            await db.SaveChangesAsync();
            return Results.Created($"/api/todo/{newTodo.Id}", newTodo);
        });

        group.MapGet("", async (TodoDbContext db) =>
            await db.TodoItems.ToListAsync());

        group.MapGet("/{id}", async (TodoDbContext db, int id) =>
        {
            var todo = await db.TodoItems.FindAsync(id);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        });

        group.MapPut("/{id}", async (TodoDbContext db, int id, TodoItem updatedTodo) =>
        {
            var todo = await db.TodoItems.FindAsync(id);
            if (todo is null) return Results.NotFound();

            todo.Title = updatedTodo.Title;
            todo.IsCompleted = updatedTodo.IsCompleted;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (TodoDbContext db, int id) =>
        {
            var todo = await db.TodoItems.FindAsync(id);
            if (todo is null) return Results.NotFound();

            db.TodoItems.Remove(todo);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return group;
    }
}
