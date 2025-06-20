using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Entities;
using Api.DTOs;

namespace Api.Interfaces
{
    public interface IIlService : IService<Il, IlDto>
    {
        Task<Il?> GetByPlaka(int plaka);
    }
}