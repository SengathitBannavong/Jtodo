using Jtodo.Domains;
using Jtodo.DTOs;
using Jtodo.Mappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jtodo.Interfaces;

namespace Jtodo.Services
{
    public class TodoListService
    {
        private readonly IAppUnit _unitOfWork;

        public TodoListService(IAppUnit unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TodoListDto>> Get_All_Todo_list_Async()
        {
            var domains = await _unitOfWork.TodoListRepository.Get_All_Todo_list_Async();
            return domains.ToDtoList();
        }

        public async Task<TodoListDto?> Get_Todo_List_Async(ulong id)
        {
            var domain = await _unitOfWork.TodoListRepository.Get_Todo_List_Async(id);
            return domain?.ToDto();
        }

        public async Task<ulong> Create_Todo_List_Async(TodoListDto dto)
        {
            var domain = dto.ToDomain();
            await _unitOfWork.TodoListRepository.Add_Todo_List_Async(domain);
            await _unitOfWork.SaveChangesAsync();
            return domain.Id;
        }

        public async Task Update_Todo_List_Async(TodoListDto dto)
        {
            var domain = dto.ToDomain();
            await _unitOfWork.TodoListRepository.Update_Todo_List_Async(domain);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Delete_Todo_List_Async(ulong id)
        {
            await _unitOfWork.TodoListRepository.Delete_Todo_List_Async(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Delete_TodoList_Complete_Async(ulong id)
        {
            await _unitOfWork.TodoListRepository.Delete_Todo_List_With_Items_Async(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

