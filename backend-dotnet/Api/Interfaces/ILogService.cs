using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs;
using Api.Entities;

namespace Api.Interfaces
{
    public interface ILogService
    {
        Task<Log> Add(Log log);
        Task<IEnumerable<Log>> GetAll();
        Task<IEnumerable<LogDto>> GetAllDto();
        Task<IEnumerable<LogDto>> GetPaginated(int skip, int take);
        Task<IEnumerable<LogDto>> GetPaginatedFiltered(int skip, int take, string filter);
        Task<int> GetCountFiltered(string filter);
        Task<Log?> GetById(int id);
        Task<int> GetCount();
        Task<IEnumerable<Log>> GetFilter(string filter);
    }
}