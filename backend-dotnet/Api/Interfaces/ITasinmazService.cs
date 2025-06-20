using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Entities;
using Api.DTOs;

namespace Api.Interfaces
{
    public interface ITasinmazService : IService<Tasinmaz, TasinmazDto>
    {
        Task<int> GetCountFiltered(string filter);
        Task<IEnumerable<TasinmazDto>> GetFilteredWithPaginationByYetki(string filter, int pageNumber, int pageSize, int kullaniciId, int yetkiId);
        Task<IEnumerable<TasinmazDto>> GetAllFiltered(string filter);
        Task<IEnumerable<TasinmazDto>> GetAllPaginatedByYetki(int skip, int take, int kullaniciId, int yetkiId);
        Task<string> GetMahalleAdiById(int mahalleId);
        Task<string> GetKullaniciAdiById(int kullaniciId);
    }
}