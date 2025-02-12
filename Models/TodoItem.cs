namespace TodoListApi.Models;

public record TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

}

