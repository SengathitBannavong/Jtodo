using Jtodo.Domains;
using Jtodo.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jtodo.Repositories
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private readonly SQLite _dbContext;

        public TodoItemRepository(SQLite dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TodoItem?> GetTodoItemAsync(ulong id)
        {
            try
            {
                return await _dbContext.TodoItems.AsNoTracking().FirstOrDefaultAsync(ti => ti.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error getting TodoItem: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TodoItem>> GetAllTodoItemsAsync()
        {
            try
            {
                return await _dbContext.TodoItems.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error getting all TodoItems: {ex.Message}");
                return new List<TodoItem>();
            }
        }

        public async Task<ulong> AddTodoItemAsync(TodoItem todoItem)
        {
            try
            {
                await _dbContext.TodoItems.AddAsync(todoItem);
                return todoItem.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error adding TodoItem: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateTodoItemAsync(TodoItem todoItem)
        {
            var trackedEntity = _dbContext.ChangeTracker.Entries<TodoItem>()
                .FirstOrDefault(e => e.Entity.Id == todoItem.Id)?.Entity;

            if (trackedEntity != null)
            {
                Console.WriteLine("[INFO] Detaching existing tracked TodoItem entity before update");
                _dbContext.Entry(trackedEntity).State = EntityState.Detached;
            }

            try
            {
                Console.WriteLine($"[INFO] Updating TodoItem ID: {todoItem.Id}");
                _dbContext.TodoItems.Update(todoItem);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error updating TodoItem: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteTodoItemAsync(ulong id)
        {
            try
            {
                var item = await _dbContext.TodoItems.FindAsync(id);
                if (item != null)
                {
                    _dbContext.TodoItems.Remove(item);
                    Console.WriteLine($"[INFO] Deleted TodoItem ID: {id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error deleting TodoItem: {ex.Message}");
                throw;
            }
        }
    }
}
