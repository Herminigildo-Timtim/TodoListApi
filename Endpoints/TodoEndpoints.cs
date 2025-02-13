using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.Models;

namespace TodoListApi.Endpoints;

public static class TodoEndpoints
{
    public static RouteGroupBuilder MapTodoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/todo");
        const string GetTodoEndpoint = "GetTodo";

        //Create new Todo
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
            return Results.CreatedAtRoute(GetTodoEndpoint, new { id = newTodo.Id }, newTodo);
        }).WithParameterValidation();

        //Get all Todo
        group.MapGet("", async (TodoDbContext db) =>
        {
            var todos = await db.TodoItems.ToListAsync();
            if (!todos.Any()) return Results.NotFound();

            return Results.Ok(todos);

        });

        group.MapDelete("/delete-all-completed", async (TodoDbContext db) =>
        {
            var completedTodos = await db.TodoItems
                .Where(t => t.IsCompleted == true)
                .ToListAsync();

            if (!completedTodos.Any())
            {
                return Results.NotFound();
            }
            db.TodoItems.RemoveRange(completedTodos);
            await db.SaveChangesAsync();

            return Results.NoContent();


        });

        //Get specific Todo ID
        group.MapGet("/{id}", async (TodoDbContext db, int id) =>
        {
            var todo = await db.TodoItems.FindAsync(id);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        }).WithName(GetTodoEndpoint);


        // Get all completed todos
        group.MapGet("/completed", async (TodoDbContext db) =>
        {
            var completedTodos = await db.TodoItems
                .Where(t => t.IsCompleted == true)
                .ToListAsync();

            if (!completedTodos.Any())
            {
                return Results.NotFound();
            }
            return Results.Ok(completedTodos);
        });

        // Toggle all todos to completed
        group.MapPut("/toggle-complete", async (TodoDbContext db, List<TodoItem> todoItems) =>
        {
            if (todoItems.Count == 0) return Results.NotFound();

            foreach (var todo in todoItems)
            {
                var todoItem = await db.TodoItems.FindAsync(todo.Id);
                if (todoItem is null) return Results.NotFound();

                todoItem.IsCompleted = todo.IsCompleted;
            }

            await db.SaveChangesAsync();

            return Results.NoContent();
        });


        // Get all remaining todos (those that are not completed)
        group.MapGet("/active", async (TodoDbContext db) =>
        {
            var remainingTodos = await db.TodoItems
                .Where(t => !t.IsCompleted)
                .ToListAsync();

            if (!remainingTodos.Any()) return Results.NotFound();

            return Results.Ok(remainingTodos);
        });


        return group;
    }
}
