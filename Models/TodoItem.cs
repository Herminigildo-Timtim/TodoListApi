using System;
using System.ComponentModel.DataAnnotations;

namespace TodoListApi.Models
{
    public record TodoItem
    {
        [Key] public int Id { get; set; }

        [Required][StringLength(100)] public string Title { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
