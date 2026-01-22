using Jtodo.Domains;
using Jtodo.Values;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jtodo.Interfaces;

namespace Jtodo.Repositories
{
    public class TodoListRepository : ITodoListRepository
    {
        private readonly SQLite _db_context;

        public TodoListRepository(SQLite db_context)
        {
            _db_context = db_context;
        }
     
        // Async methods
        public async Task<TodoList?> Get_Todo_List_Async(ulong id)
        {
            try
            {
                Console.WriteLine($"[INFO] Querying TodoList async with ID: {id}");
                
                var todoList = await _db_context.TodoLists
                    .AsNoTracking()
                    .FirstOrDefaultAsync(tl => tl.Id == id);
                
                if (todoList != null)
                {
                    await LoadTodoItemsAsync(todoList);
                }
                
                return todoList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error querying TodoList async: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TodoList>> Get_All_Todo_list_Async()
        {
            try
            {
                Console.WriteLine("[INFO] Querying all TodoLists async from database...");
                
                var todoLists = await _db_context.TodoLists
                    .AsNoTracking()
                    .ToListAsync();
                
                Console.WriteLine($"[INFO] Found {todoLists.Count} TodoList(s) in database");
                
                foreach (var todoList in todoLists)
                {
                    await LoadTodoItemsAsync(todoList);
                }
                
                return todoLists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error querying TodoLists async: {ex.Message}");
                return new List<TodoList>();
            }
        }

        public async Task<ulong> Add_Todo_List_Async(TodoList todoList)
        {
            try
            {
                Console.WriteLine($"[INFO] Adding TodoList async: {todoList.Title}");
                
                await _db_context.TodoLists.AddAsync(todoList);
                
                return todoList.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error adding TodoList async: {ex.Message}");
                throw;
            }
        }

        public async Task Update_Todo_List_Async(TodoList todoList)
        {
            var trackedEntity = _db_context.ChangeTracker.Entries<TodoList>()
                .FirstOrDefault(e => e.Entity.Id == todoList.Id)?.Entity;

            if (trackedEntity != null)
            {
                Console.WriteLine("[INFO] Detaching existing tracked TodoList entity before update");
                _db_context.Entry(trackedEntity).State = EntityState.Detached;
            }
       
            try
            {
                Console.WriteLine($"[INFO] Updating TodoList async: {todoList.Title}");
                _db_context.TodoLists.Update(todoList);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error updating TodoList async: {ex.Message}");
                throw;
            }
        }

        public async Task Delete_Todo_List_Async(ulong id)
        {
            try
            {
                var list = await _db_context.TodoLists.FindAsync(id);
                if (list != null)
                {
                    _db_context.TodoLists.Remove(list);
                    Console.WriteLine($"[INFO] Deleted TodoList async ID: {id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error deleting TodoList async: {ex.Message}");
                throw;
            }
        }
        public async Task Delete_Todo_List_With_Items_Async(ulong id)
        {
            try
            {
                Console.WriteLine($"[INFO] Deleting TodoList with items async ID: {id}");

                // 1. Get TodoItem IDs have relation with TodoList
                var todoItemIds = await _db_context.TodoListItems
                    .Where(tli => tli.TodoListId == id)
                    .Select(tli => tli.TodoItemId)
                    .ToListAsync();

                if (todoItemIds.Any())
                {
                    // 2. junction records
                    var junctionRecords = await _db_context.TodoListItems
                        .Where(tli => tli.TodoListId == id)
                        .ToListAsync();
                    
                    _db_context.TodoListItems.RemoveRange(junctionRecords);
                    Console.WriteLine($"[INFO] Removed {junctionRecords.Count} junction records");

                    // 3. delete TodoItems
                    var todoItems = await _db_context.TodoItems
                        .Where(ti => todoItemIds.Contains(ti.Id))
                        .ToListAsync();
                    
                    _db_context.TodoItems.RemoveRange(todoItems);
                    Console.WriteLine($"[INFO] Removed {todoItems.Count} TodoItems");
                }

                // 4. delete TodoList
                var todoList = await _db_context.TodoLists.FindAsync(id);
                if (todoList != null)
                {
                    _db_context.TodoLists.Remove(todoList);
                    Console.WriteLine($"[INFO] Removed TodoList ID: {id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error deleting TodoList with items: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> Exists_Async(ulong id)
        {
            return await _db_context.TodoLists.AnyAsync(tl => tl.Id == id);
        }

        // Helper methods
        private async Task LoadTodoItemsAsync(TodoList todoList)
        {
            var todoItemIds = await _db_context.TodoListItems
                .Where(tli => tli.TodoListId == todoList.Id)
                .Select(tli => tli.TodoItemId)
                .ToListAsync();
            
            var todoItems = await _db_context.TodoItems
                .Where(ti => todoItemIds.Contains(ti.Id))
                .ToListAsync();
            
            foreach (var item in todoItems)
            {
                todoList.Add_Todo_Item(item);
            }
        }
    }
}
