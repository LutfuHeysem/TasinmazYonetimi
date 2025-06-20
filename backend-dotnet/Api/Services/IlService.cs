using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Interfaces;
using Api.Data;
using Api.Entities;
using Microsoft.EntityFrameworkCore;
using Api.DTOs;

namespace Api.Services
{
    public class IlService : IIlService
    {
        private readonly ApplicationDbContext _context;

        public IlService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IlDto> Add(Il entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _context.Iller.Add(entity);
            await _context.SaveChangesAsync();
            return new IlDto
            {
                Id = entity.Id,
                IlAdi = entity.IlAdi,
                Plaka = entity.Plaka
            };
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Iller.FindAsync(id);
            if (entity != null)
            {
                _context.Iller.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Il with id {id} not found.");
            }
        }
        public async Task<IEnumerable<IlDto>> GetAll()
        {
            var iller = await _context.Iller.OrderBy(il => il.IlAdi).ToListAsync();
            var ilDtos = new List<IlDto>();
            foreach (var il in iller)
            {
                ilDtos.Add(new IlDto
                {
                    Id = il.Id,
                    IlAdi = il.IlAdi,
                    Plaka = il.Plaka
                });
            }
            return ilDtos; 
        }

        public async Task<Il> GetById(int id)
        {
            var il = await _context.Iller.FirstOrDefaultAsync(il => il.Id == id)
                   ?? throw new KeyNotFoundException($"Il with id {id} not found.");
            return il;
        }

        public Task<Il?> GetByPlaka(int plaka)
        {
            return _context.Iller.FirstOrDefaultAsync(il => il.Plaka == plaka);
        }

        public async Task<int> GetCount()
        {
            return await _context.Iller.CountAsync();
        }

        public async Task<IEnumerable<IlDto>> GetPaginated(int skip, int take)
        {
            var iller = await _context.Iller
                .OrderBy(il => il.IlAdi)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var ilDtos = new List<IlDto>();
            foreach (var il in iller)
            {
                ilDtos.Add(new IlDto
                {
                    Id = il.Id,
                    IlAdi = il.IlAdi,
                    Plaka = il.Plaka
                });
            }
            return ilDtos;
        }

        public async Task<IlDto> Update(IlDto entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var il = await _context.Iller.FindAsync(entity.Id)
                ?? throw new KeyNotFoundException($"Il with id {entity.Id} not found.");
            il.IlAdi = entity.IlAdi;
            await _context.SaveChangesAsync();
            return new IlDto
            {
                Id = il.Id,
                IlAdi = il.IlAdi,
                Plaka = il.Plaka
            };
        }
    }
}