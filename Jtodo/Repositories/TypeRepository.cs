using Jtodo.Domains;
using Jtodo.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jtodo.Repositories
{
    public class TypeRepository : ITypeRepository
    {
        private readonly SQLite _dbContext;

        public TypeRepository(SQLite dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Domains.Type>> GetAllTypesAsync()
        {
            try
            {
                Console.WriteLine("[INFO] Querying all Types from database...");
                var types = await _dbContext.Types.AsNoTracking().ToListAsync();
                Console.WriteLine($"[INFO] Found {types.Count} Type(s) in database");
                return types;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error querying Types: {ex.Message}");
                return new List<Domains.Type>();
            }
        }

        public async Task<Domains.Type?> GetTypeByIdAsync(ulong id)
        {
            try
            {
                return await _dbContext.Types.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error querying Type by ID: {ex.Message}");
                return null;
            }
        }

        public async Task CreateTypeAsync(Domains.Type type)
        {
            try
            {
                Console.WriteLine($"[INFO] Creating new Type: {type.Text}");
                await _dbContext.Types.AddAsync(type);
                Console.WriteLine("[INFO] Type created successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error creating Type: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateTypeAsync(Domains.Type type)
        {
            var trackedEntity = _dbContext.ChangeTracker.Entries<Domains.Type>()
                .FirstOrDefault(e => e.Entity.Id == type.Id)?.Entity;

            if (trackedEntity != null)
            {
                Console.WriteLine("[INFO] Detaching existing tracked Type entity before update");
                _dbContext.Entry(trackedEntity).State = EntityState.Detached;
            }

            try
            {
                Console.WriteLine($"[INFO] Updating Type ID: {type.Id}");
                _dbContext.Types.Update(type);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error updating Type: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteTypeAsync(ulong id)
        {
            try
            {
                var type = await _dbContext.Types.FindAsync(id);
                if (type != null)
                {
                    _dbContext.Types.Remove(type);
                    Console.WriteLine($"[INFO] Deleted Type ID: {id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error deleting Type: {ex.Message}");
                throw;
            }
        }
    }
}

