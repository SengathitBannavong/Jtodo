using Jtodo.Domains;
using Jtodo.Values;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jtodo.Repositories
{
    public class TodoListRespository
    {
        private readonly SQLite _db_context;

        public TodoListRespository(SQLite db_context)
        {
            _db_context = db_context;
        }

        public void CheckDatabaseStructure()
        {
            try
            {
                Console.WriteLine("[INFO] Checking database structure...");
                Console.WriteLine();
                
                Console.WriteLine("[INFO] Entities configured in DbContext:");
                Console.WriteLine($"  - TodoLists: {_db_context.TodoLists.EntityType.Name}");
                Console.WriteLine($"  - TodoItems: {_db_context.TodoItems.EntityType.Name}");
                Console.WriteLine($"  - TodoListItems: {_db_context.TodoListItems.EntityType.Name}");
                Console.WriteLine($"  - Types: {_db_context.Types.EntityType.Name}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error checking database: {ex.Message}");
                Console.WriteLine();
            }
        }

        public TodoList? Get_Todo_List(UInt64 id)
        {
            try
            {
                Console.WriteLine($"[INFO] Querying TodoList with ID: {id}");
                
                var todoList = _db_context.TodoLists
                    .AsNoTracking()
                    .FirstOrDefault(tl => tl.Id == id);
                
                if (todoList != null)
                {
                    // Load related TodoItems via junction table
                    var todoItemIds = _db_context.TodoListItems
                        .Where(tli => tli.TodoListId == id)
                        .Select(tli => tli.TodoItemId)
                        .ToList();
                    
                    var todoItems = _db_context.TodoItems
                        .Where(ti => todoItemIds.Contains(ti.Id))
                        .ToList();
                    
                    foreach (var item in todoItems)
                    {
                        todoList.Add_Todo_Item(item);
                    }
                }
                
                return todoList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error querying TodoList: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        public List<TodoList> Get_All_Todo_list()
        {
            try
            {
                Console.WriteLine("[INFO] Querying all TodoLists from database...");
                Console.WriteLine();
                
                // Query TodoLists from database
                var todoLists = _db_context.TodoLists
                    .AsNoTracking()
                    .ToList();
                
                Console.WriteLine($" [INFO] Found {todoLists.Count} TodoList(s) in database");
                
                // Load related TodoItems for each TodoList
                foreach (var todoList in todoLists)
                {
                    var todoItemIds = _db_context.TodoListItems
                        .Where(tli => tli.TodoListId == todoList.Id)
                        .Select(tli => tli.TodoItemId)
                        .ToList();
                    
                    var todoItems = _db_context.TodoItems
                        .Where(ti => todoItemIds.Contains(ti.Id))
                        .ToList();
                    
                    foreach (var item in todoItems)
                    {
                        todoList.Add_Todo_Item(item);
                    }
                    
                    Console.WriteLine($"  [INFO] Loaded TodoList ID {todoList.Id}: {todoList.Title} ({todoList.Todo_Items.Count} items)");
                }
                
                Console.WriteLine();
                return todoLists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error querying TodoLists: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return new List<TodoList>();
            }
        }
    }
}
