using Jtodo.Domains;
using Jtodo.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jtodo.Mappers
{
    public static class TodoItemMapper
    {
        public static TodoItemDto ToDto(this TodoItem domain)
        {
            return new TodoItemDto(
                id: domain.Id,
                title: domain.Title,
                description: domain.Description,
                startDate: domain.StartDate ?? DateTime.Now,
                dueDate: domain.DueDate ?? DateTime.Now.AddDays(7),
                priority: domain.Priority,
                status: domain.Status,
                typeId: domain.TypeId
            );
        }
        public static TodoItem ToDomain(this TodoItemDto dto)
        {
            return new TodoItem(
                Id: dto.Id,
                Title: dto.Title,
                Description: dto.Description,
                StartDate: dto.StartDate,
                DueDate: dto.DueDate,
                Priority: dto.Priority,
                Status: dto.Status,
                TypeId: dto.TypeId
            );
        }

        public static List<TodoItemDto> ToDtoList(this IEnumerable<TodoItem> domains)
        {
            return domains.Select(d => d.ToDto()).ToList();
        }

        public static List<TodoItem> ToDomainList(this IEnumerable<TodoItemDto> dtos)
        {
            return dtos.Select(d => d.ToDomain()).ToList();
        }
    }
}
