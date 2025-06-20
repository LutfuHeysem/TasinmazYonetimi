using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IService<Tentity, TDto> where Tentity : class where TDto : class
    {
        Task<Tentity> GetById(int id);
        Task<IEnumerable<TDto>> GetAll();
        Task<IEnumerable<TDto>> GetPaginated(int skip, int take);
        Task<int> GetCount();
        Task<TDto> Add(Tentity entity);
        Task Delete(int id);
        Task<TDto> Update(TDto entityDto);
    }
}