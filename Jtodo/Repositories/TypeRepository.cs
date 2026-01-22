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
    }
}
