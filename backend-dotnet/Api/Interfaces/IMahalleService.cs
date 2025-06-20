using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Entities;
using Api.DTOs;

namespace Api.Interfaces
{
    public interface IMahalleService : IService<Mahalle, MahalleDto>
    {
        Task<string> GetIlceAdiById(int ilceId);
    }
}