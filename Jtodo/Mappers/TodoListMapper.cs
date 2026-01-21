using Jtodo.Domains;
using Jtodo.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Jtodo.Mappers
{
    public static class TodoListMapper
    {

        public static TodoListDto ToDto(this TodoList domain)
        {
            var dto = new TodoListDto
            {
                Id = domain.Id,
                Title = domain.Title,
                Description = domain.Description,
                CreateDate = domain.CreateDate,
                TodoItems = domain.Todo_Items?.Select(item => item.ToDto()).ToList() ?? new List<TodoItemDto>()
            };
            
            return dto;
        }
        
        public static TodoList ToDomain(this TodoListDto dto)
        {
            var domain = new TodoList(
                Id: dto.Id,
                Title: dto.Title,
                Description: dto.Description,
                CreateDate: dto.CreateDate
            );
            
            if (dto.TodoItems != null)
            {
                foreach (var itemDto in dto.TodoItems)
                {
                    var itemDomain = itemDto.ToDomain();
                    domain.Add_Todo_Item(itemDomain);
                }
            }
            
            return domain;
        }

        public static List<TodoListDto> ToDtoList(this IEnumerable<TodoList> domains)
        {
            return domains.Select(d => d.ToDto()).ToList();
        }
        
        public static List<TodoList> ToDomainList(this IEnumerable<TodoListDto> dtos)
        {
            return dtos.Select(d => d.ToDomain()).ToList();
        }
    }
}

