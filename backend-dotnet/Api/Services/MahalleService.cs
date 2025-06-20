using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Interfaces;
using Api.DTOs;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Entities;

namespace Api.Services
{
    public class MahalleService : IMahalleService
    {
        private readonly ApplicationDbContext _context;

        public MahalleService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<MahalleDto> Add(Mahalle entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Mahalleler.Add(entity);
            await _context.SaveChangesAsync();
            return new MahalleDto
            {
                Id = entity.Id,
                MahalleAdi = entity.MahalleAdi,
                IlceId = entity.IlceId,
                IlceAdi = await _context.Ilceler
                    .Where(ilce => ilce.Id == entity.IlceId)
                    .Select(ilce => ilce.IlceAdi)
                    .FirstOrDefaultAsync() 
                    ?? throw new KeyNotFoundException($"Ilce with id {entity.IlceId} not found.")
            };
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Mahalleler.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException("Mahalle not found with the specified ID.");
            _context.Mahalleler.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MahalleDto>> GetAll()
        {
            var mahalleler = await _context.Mahalleler.OrderBy(mahalle => mahalle.MahalleAdi).ToListAsync();
            var mahallelerDto = new List<MahalleDto>();
            foreach (var mahalle in mahalleler)
            {
                var ilce = await _context.Ilceler.FindAsync(mahalle.IlceId);
                if (ilce == null)
                {
                    throw new KeyNotFoundException($"Ilce with id {mahalle.IlceId} not found.");
                }
                mahallelerDto.Add(new MahalleDto
                {
                    Id = mahalle.Id,
                    MahalleAdi = mahalle.MahalleAdi,
                    IlceId = mahalle.IlceId,
                    IlceAdi = ilce.IlceAdi
                });
            }
            return mahallelerDto;
        }

        public async Task<Mahalle> GetById(int id)
        {
            var mahalle = await _context.Mahalleler.FindAsync(id)
                   ?? throw new KeyNotFoundException($"Mahalle with id {id} not found.");

            mahalle.Ilce = await _context.Ilceler.FindAsync(mahalle.IlceId)
                ?? throw new KeyNotFoundException($"Ilce with id {mahalle.IlceId} not found.");
            return mahalle;
        }

        public async Task<int> GetCount()
        {
            return await _context.Mahalleler.CountAsync();
        }

        public async Task<string> GetIlceAdiById(int ilceId)
        {
            return await _context.Ilceler
                .Where(ilce => ilce.Id == ilceId)
                .Select(ilce => ilce.IlceAdi)
                .FirstOrDefaultAsync() 
                ?? throw new KeyNotFoundException($"Ilce with id {ilceId} not found.");
        }

        public async Task<IEnumerable<MahalleDto>> GetPaginated(int skip, int take)
        {
            var mahalleler = await _context.Mahalleler
                .OrderBy(mahalle => mahalle.MahalleAdi)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var mahallelerDto = new List<MahalleDto>();
            foreach (var mahalle in mahalleler)
            {
                var ilce = await _context.Ilceler.FindAsync(mahalle.IlceId);
                if (ilce == null)
                {
                    throw new KeyNotFoundException($"Ilce with id {mahalle.IlceId} not found.");
                }
                mahallelerDto.Add(new MahalleDto
                {
                    Id = mahalle.Id,
                    MahalleAdi = mahalle.MahalleAdi,
                    IlceId = mahalle.IlceId,
                    IlceAdi = ilce.IlceAdi
                });
            }
            return mahallelerDto;
        }

        public async Task<MahalleDto> Update(MahalleDto entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var mahalle = await _context.Mahalleler.FindAsync(entity.Id)
                ?? throw new KeyNotFoundException($"Mahalle with id {entity.Id} not found.");
            mahalle.MahalleAdi = entity.MahalleAdi;
            mahalle.IlceId = entity.IlceId;
            mahalle.Ilce = await _context.Ilceler.FindAsync(entity.IlceId)
                ?? throw new KeyNotFoundException($"Ilce with id {entity.IlceId} not found.");
            await _context.SaveChangesAsync();
            return new MahalleDto
            {
                Id = mahalle.Id,
                MahalleAdi = mahalle.MahalleAdi,
                IlceId = mahalle.IlceId,
                IlceAdi = mahalle.Ilce.IlceAdi
            };
        }
    }
}