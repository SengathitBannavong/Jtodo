using Jtodo.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jtodo.Services
{
    public class TypeService
    {
        private readonly IAppUnit _unitOfWork;

        public TypeService(IAppUnit unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Domains.Type>> GetAllTypesAsync()
        {
            return await _unitOfWork.TypeRepository.GetAllTypesAsync();
        }

        public async Task<Domains.Type?> GetTypeByIdAsync(ulong id)
        {
            return await _unitOfWork.TypeRepository.GetTypeByIdAsync(id);
        }

        public async Task<Domains.Type> CreateTypeAsync(Domains.Type type)
        {
            await _unitOfWork.TypeRepository.CreateTypeAsync(type);
            await _unitOfWork.SaveChangesAsync();
            return type;
        }

        public async Task UpdateTypeAsync(Domains.Type type)
        {
            await _unitOfWork.TypeRepository.UpdateTypeAsync(type);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTypeAsync(ulong id)
        {
            await _unitOfWork.TypeRepository.DeleteTypeAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ulong> GetNoneTypeIdAsync()
        {
            var types = await _unitOfWork.TypeRepository.GetAllTypesAsync();
            var noneType = types.FirstOrDefault(t => t.Text == "None");
            
            if (noneType == null)
            {
                // If "None" type doesn't exist, create it
                var dbContext = _unitOfWork.GetDbContext();
                await dbContext.EnsureDefaultTypeExistsAsync();
                
                // Retrieve again
                types = await _unitOfWork.TypeRepository.GetAllTypesAsync();
                noneType = types.FirstOrDefault(t => t.Text == "None");
            }
            
            return noneType?.Id ?? 1; // Fallback to ID 1 if still not found
        }
    }
}

