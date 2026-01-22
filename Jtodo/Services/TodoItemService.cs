using Jtodo.DTOs;
using Jtodo.Interfaces;
using Jtodo.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jtodo.Services
{
    public class TodoItemService
    {
        private readonly IAppUnit _unitOfWork;

        // Event for notifying when data changes
        public event EventHandler? DataChanged;

        public TodoItemService(IAppUnit unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Method to trigger the event
        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
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
            
            OnDataChanged(); // Notify subscribers
        }

        public async Task DeleteTodoItemAsync(ulong id)
        {
            await _unitOfWork.TodoItemRepository.DeleteTodoItemAsync(id);
            await _unitOfWork.SaveChangesAsync();
            
            OnDataChanged(); // Notify subscribers
        }
    }
}
