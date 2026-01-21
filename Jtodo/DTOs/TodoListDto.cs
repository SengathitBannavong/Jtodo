using System;
using System.Collections.Generic;

namespace Jtodo.DTOs
{
    public class TodoListDto
    {
        public ulong Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public List<TodoItemDto> TodoItems { get; set; }

        public TodoListDto()
        {
            Title = string.Empty;
            Description = string.Empty;
            CreateDate = DateTime.Now;
            TodoItems = new List<TodoItemDto>();
        }

        public TodoListDto(string title, string description)
        {
            Id = 0;
            Title = title;
            Description = description;
            CreateDate = DateTime.Now;
            TodoItems = new List<TodoItemDto>();
        }

        public TodoListDto(ulong id, string title, string description, DateTime createDate)
        {
            Id = id;
            Title = title;
            Description = description;
            CreateDate = createDate;
            TodoItems = new List<TodoItemDto>();
        }
        
        public TodoListDto(ulong id, string title, string description, DateTime createDate, List<TodoItemDto> todoItems)
        {
            Id = id;
            Title = title;
            Description = description;
            CreateDate = createDate;
            TodoItems = todoItems ?? new List<TodoItemDto>();
        }
    }
}
