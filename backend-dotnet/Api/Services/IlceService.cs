using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Interfaces;
using Api.DTOs;
using Api.Data;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class IlceService : IIlceService
    {
        private readonly ApplicationDbContext _context;

        public IlceService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IlceDto> Add(Ilce entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _context.Ilceler.Add(entity);
            await _context.SaveChangesAsync();
            return new IlceDto
            {
                Id = entity.Id,
                IlceAdi = entity.IlceAdi,
                IlId = entity.IlId,
                IlAdi = (await _context.Iller.FindAsync(entity.IlId))?.IlAdi ?? string.Empty
            };
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Ilceler.FindAsync(id);
            if (entity != null)
            {
                _context.Ilceler.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Ilce with id {id} not found.");
            }
        }

        public async Task<IEnumerable<IlceDto>> GetAll()
        {
            var ilceler = await _context.Ilceler.OrderBy(ilce => ilce.IlceAdi).ToListAsync();
            var ilceDtos = new List<IlceDto>();
            foreach (var ilce in ilceler)
            {
                var il = await _context.Iller.FindAsync(ilce.IlId);
                ilceDtos.Add(new IlceDto
                {
                    Id = ilce.Id,
                    IlceAdi = ilce.IlceAdi,
                    IlId = ilce.IlId,
                    IlAdi = il?.IlAdi ?? string.Empty
                });
            }
            return ilceDtos;
        }

        public async Task<Ilce> GetById(int id)
        {
            var ilce = await _context.Ilceler.FindAsync(id)
                   ?? throw new KeyNotFoundException($"Ilce with id {id} not found.");
            ilce.Il = await _context.Iller.FindAsync(ilce.IlId)
                   ?? throw new KeyNotFoundException($"Il with id {ilce.IlId} not found.");
            return ilce;
        }

        public async Task<int> GetCount()
        {
            return await _context.Ilceler.CountAsync();
        }
        public async Task<IEnumerable<IlceDto>> GetPaginated(int skip, int take)
        {
            var ilceler = await _context.Ilceler
                .OrderBy(ilce => ilce.IlceAdi)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var ilceDtos = new List<IlceDto>();
            foreach (var ilce in ilceler)
            {
                var il = await _context.Iller.FindAsync(ilce.IlId);
                ilceDtos.Add(new IlceDto
                {
                    Id = ilce.Id,
                    IlceAdi = ilce.IlceAdi,
                    IlId = ilce.IlId,
                    IlAdi = il?.IlAdi ?? string.Empty
                });
            }
            return ilceDtos;
        }

        public async Task<IlceDto> Update(IlceDto ilceDto)
        {
            ArgumentNullException.ThrowIfNull(ilceDto);
            var entity = await _context.Ilceler.FindAsync(ilceDto.Id)
                ?? throw new KeyNotFoundException($"Ilce with id {ilceDto.Id} not found.");
            entity.IlceAdi = ilceDto.IlceAdi;
            var il = await _context.Iller.FindAsync(ilceDto.IlId)
                ?? throw new KeyNotFoundException($"Il with id {ilceDto.IlId} not found.");
            entity.Il = il;
            entity.IlId = il.Id;
            await _context.SaveChangesAsync();
            return new IlceDto
            {
                Id = entity.Id,
                IlceAdi = entity.IlceAdi,
                IlId = entity.IlId,
                IlAdi = il.IlAdi
            };
        }
    }
}