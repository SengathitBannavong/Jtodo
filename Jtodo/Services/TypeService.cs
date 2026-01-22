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
    }
}
