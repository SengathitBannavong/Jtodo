using Jtodo.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jtodo.Interfaces
{
    public interface ITypeRepository
    {
        Task<List<Domains.Type>> GetAllTypesAsync();
        Task<Domains.Type?> GetTypeByIdAsync(ulong id);
    }
}
