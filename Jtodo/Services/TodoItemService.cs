using Jtodo.DTOs;
using Jtodo.Interfaces;
using Jtodo.Mappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jtodo.Services
{
    public class TodoItemService
    {
        private readonly IAppUnit _unitOfWork;

        public TodoItemService(IAppUnit unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TodoItemDto?> GetTodoItemAsync(ulong id)
        {
            var domain = await _unitOfWork.TodoItemRepository.GetTodoItemAsync(id);
            return domain?.ToDto();
        }

        public async Task<List<TodoItemDto>> GetAllTodoItemsAsync()
        {
            var domains = await _unitOfWork.TodoItemRepository.GetAllTodoItemsAsync();
            return domains.Select(d => d.ToDto()).ToList();
        }

        public async Task<ulong> CreateTodoItemAsync(TodoItemDto dto)
        {
            var domain = dto.ToDomain();
            await _unitOfWork.TodoItemRepository.AddTodoItemAsync(domain);
            await _unitOfWork.SaveChangesAsync();
            return domain.Id;
        }

        public async Task UpdateTodoItemAsync(TodoItemDto dto)
        {
            var domain = dto.ToDomain();
            await _unitOfWork.TodoItemRepository.UpdateTodoItemAsync(domain);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTodoItemAsync(ulong id)
        {
            await _unitOfWork.TodoItemRepository.DeleteTodoItemAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
