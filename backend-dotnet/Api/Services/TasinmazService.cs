using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Interfaces;
using Api.Data;
using Api.Entities;
using Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class TasinmazService : ITasinmazService
    {
        private readonly ApplicationDbContext _context;

        public TasinmazService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TasinmazDto> Add(Tasinmaz entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Tasinmazlar.Add(entity);
            await _context.SaveChangesAsync();
            var mahalle = await _context.Mahalleler.FindAsync(entity.MahalleId);
            var ilce = await _context.Ilceler.FindAsync(mahalle?.IlceId);
            var il = await _context.Iller.FindAsync(ilce?.IlId);
            var tasinmazDto = new TasinmazDto()
            {
                Id = entity.Id,
                Ada = entity.Ada,
                Parsel = entity.Parsel,
                Nitelik = entity.Nitelik,
                KoordinatBilgileri = entity.KoordinatBilgileri,
                MahalleId = entity.MahalleId,
                KullaniciId = entity.KullaniciId,
                KullaniciAdi = (await _context.Kullanicilar.FindAsync(entity.KullaniciId))?.Ad + " " + (await _context.Kullanicilar.FindAsync(entity.KullaniciId))?.Soyad,
                MahalleAdi = mahalle?.MahalleAdi ?? string.Empty,
                IlAdi = il?.IlAdi ?? string.Empty,
                IlceAdi = ilce?.IlceAdi ?? string.Empty
            };
            return tasinmazDto;
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Tasinmazlar.FindAsync(id);
            if (entity != null)
            {
                _context.Tasinmazlar.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Tasinmaz with id {id} not found.");
            }
        }

        public async Task<IEnumerable<TasinmazDto>> GetAll()
        {
            var tasinmazlar = await _context.Tasinmazlar.ToListAsync();
            var tasinmazlarDto = new List<TasinmazDto>();
            foreach (var tasinmaz in tasinmazlar)
            {
                var mahalle = await _context.Mahalleler.FindAsync(tasinmaz.MahalleId);
                if (mahalle == null)
                {
                    throw new KeyNotFoundException($"Mahalle with id {tasinmaz.MahalleId} not found.");
                }
                var kullanici = await _context.Kullanicilar.FindAsync(tasinmaz.KullaniciId);
                if (kullanici == null)
                {
                    throw new KeyNotFoundException($"Kullanici with id {tasinmaz.KullaniciId} not found.");
                }
                var ilce = await _context.Ilceler.FindAsync(mahalle.IlceId);
                var il = await _context.Iller.FindAsync(ilce?.IlId);
                tasinmazlarDto.Add(new TasinmazDto
                {
                    Id = tasinmaz.Id,
                    Ada = tasinmaz.Ada,
                    Parsel = tasinmaz.Parsel,
                    Nitelik = tasinmaz.Nitelik,
                    KoordinatBilgileri = tasinmaz.KoordinatBilgileri,
                    IlAdi = il?.IlAdi ?? string.Empty,
                    IlceAdi = ilce?.IlceAdi ?? string.Empty,
                    MahalleId = tasinmaz.MahalleId,
                    MahalleAdi = mahalle.MahalleAdi,
                    KullaniciId = tasinmaz.KullaniciId,
                    KullaniciAdi = kullanici.Ad + " " + kullanici.Soyad
                });
            }
            return tasinmazlarDto;
        }

        public async Task<IEnumerable<TasinmazDto>> GetAllFiltered(string filter)
        {
            var tasinmazlar = await (from t in _context.Tasinmazlar
                                     where t.Mahalle.MahalleAdi.Contains(filter) ||
                                           t.Ada.Contains(filter) ||
                                           t.Parsel.Contains(filter) ||
                                           t.Nitelik.Contains(filter) ||
                                           t.KoordinatBilgileri.Contains(filter)
                                     select t).OrderBy(t => t.KoordinatBilgileri).ToListAsync();
            var tasinmazlarDto = new List<TasinmazDto>();
            foreach (var tasinmaz in tasinmazlar)
            {
                var mahalle = await _context.Mahalleler.FindAsync(tasinmaz.MahalleId);
                if (mahalle == null)
                {
                    throw new KeyNotFoundException($"Mahalle with id {tasinmaz.MahalleId} not found.");
                }
                var kullanici = await _context.Kullanicilar.FindAsync(tasinmaz.KullaniciId);
                if (kullanici == null)
                {
                    throw new KeyNotFoundException($"Kullanici with id {tasinmaz.KullaniciId} not found.");
                }
                var ilce = await _context.Ilceler.FindAsync(mahalle.IlceId);
                var il = await _context.Iller.FindAsync(ilce?.IlId);
                tasinmazlarDto.Add(new TasinmazDto
                {
                    Id = tasinmaz.Id,
                    Ada = tasinmaz.Ada,
                    Parsel = tasinmaz.Parsel,
                    Nitelik = tasinmaz.Nitelik,
                    KoordinatBilgileri = tasinmaz.KoordinatBilgileri,
                    MahalleId = tasinmaz.MahalleId,
                    MahalleAdi = mahalle.MahalleAdi,
                    KullaniciId = tasinmaz.KullaniciId,
                    KullaniciAdi = kullanici.Ad + " " + kullanici.Soyad,
                    IlAdi = il?.IlAdi ?? string.Empty,
                    IlceAdi = ilce?.IlceAdi ?? string.Empty
                });
            }
            return tasinmazlarDto;
        }
        public async Task<IEnumerable<TasinmazDto>> GetAllPaginatedByYetki(int skip, int take, int kullaniciId, int yetkiId)
        {
            var tasinmazlar = new List<Tasinmaz>();
            if (yetkiId == 1) // Admin yetkisi
            {
                tasinmazlar = await _context.Tasinmazlar
                    .OrderBy(t => t.KoordinatBilgileri)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();
            }
            else // Kullanıcı yetkisi
            {
                tasinmazlar = await _context.Tasinmazlar
                    .Where(t => t.KullaniciId == kullaniciId)
                    .OrderBy(t => t.KoordinatBilgileri)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();
            }
            var tasinmazlarDto = new List<TasinmazDto>();
            foreach (var tasinmaz in tasinmazlar)
            {
                var mahalle = await _context.Mahalleler.FindAsync(tasinmaz.MahalleId);
                if (mahalle == null)
                {
                    throw new KeyNotFoundException($"Mahalle with id {tasinmaz.MahalleId} not found.");
                }
                var kullanici = await _context.Kullanicilar.FindAsync(tasinmaz.KullaniciId);
                if (kullanici == null)
                {
                    throw new KeyNotFoundException($"Kullanici with id {tasinmaz.KullaniciId} not found.");
                }
                var ilce = await _context.Ilceler.FindAsync(mahalle.IlceId);
                var il = await _context.Iller.FindAsync(ilce?.IlId);
                tasinmazlarDto.Add(new TasinmazDto
                {
                    Id = tasinmaz.Id,
                    Ada = tasinmaz.Ada,
                    Parsel = tasinmaz.Parsel,
                    Nitelik = tasinmaz.Nitelik,
                    KoordinatBilgileri = tasinmaz.KoordinatBilgileri,
                    MahalleId = tasinmaz.MahalleId,
                    MahalleAdi = mahalle.MahalleAdi,
                    KullaniciId = tasinmaz.KullaniciId,
                    KullaniciAdi = kullanici.Ad + " " + kullanici.Soyad,
                    IlAdi = il?.IlAdi ?? string.Empty,
                    IlceAdi = ilce?.IlceAdi ?? string.Empty
                });
            }
            return tasinmazlarDto;
        }

        public async Task<Tasinmaz> GetById(int id)
        {
            var tasinmaz = await _context.Tasinmazlar.FindAsync(id)
                   ?? throw new KeyNotFoundException($"Tasinmaz with id {id} not found.");
            return tasinmaz;
        }

        public async Task<int> GetCount()
        {
            return await _context.Tasinmazlar.CountAsync();
        }

        public async Task<int> GetCountFiltered(string filter)
        {
            return await _context.Tasinmazlar
                .CountAsync(t => t.Mahalle.MahalleAdi.Contains(filter) ||
                                 t.Ada.Contains(filter) ||
                                 t.Parsel.Contains(filter) ||
                                 t.Nitelik.Contains(filter));
        }

        public async Task<IEnumerable<TasinmazDto>> GetFilteredWithPaginationByYetki(string filter, int pageNumber, int pageSize, int kullaniciId, int yetkiId)
        {
            filter = filter?.Trim() ?? string.Empty;
            filter = filter.ToLower();
            var tasinmazlar = new List<Tasinmaz>();
            if (yetkiId == 1) // Admin yetkisi
            {
                tasinmazlar = await _context.Tasinmazlar
                    .Where(t => t.Mahalle.MahalleAdi.ToLower().Contains(filter) ||
                                 t.Ada.ToLower().Contains(filter) ||
                                 t.Parsel.ToLower().Contains(filter) ||
                                 t.Nitelik.ToLower().Contains(filter))
                    .OrderBy(t => t.KoordinatBilgileri)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else // Kullanıcı yetkisi
            {
                tasinmazlar = await _context.Tasinmazlar
                    .Where(t => t.KullaniciId == kullaniciId &&
                                 (t.Mahalle.MahalleAdi.ToLower().Contains(filter) ||
                                  t.Ada.ToLower().Contains(filter) ||
                                  t.Parsel.ToLower().Contains(filter) ||
                                  t.Nitelik.ToLower().Contains(filter)))
                    .OrderBy(t => t.KoordinatBilgileri)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            var tasinmazlarDto = new List<TasinmazDto>();
            foreach (var tasinmaz in tasinmazlar)
            {
                var mahalle = await _context.Mahalleler.FindAsync(tasinmaz.MahalleId);
                if (mahalle == null)
                {
                    throw new KeyNotFoundException($"Mahalle with id {tasinmaz.MahalleId} not found.");
                }
                var kullanici = await _context.Kullanicilar.FindAsync(tasinmaz.KullaniciId);
                if (kullanici == null)
                {
                    throw new KeyNotFoundException($"Kullanici with id {tasinmaz.KullaniciId} not found.");
                }
                var ilce = await _context.Ilceler.FindAsync(mahalle.IlceId);
                var il = await _context.Iller.FindAsync(ilce?.IlId);
                tasinmazlarDto.Add(new TasinmazDto
                {
                    Id = tasinmaz.Id,
                    Ada = tasinmaz.Ada,
                    Parsel = tasinmaz.Parsel,
                    Nitelik = tasinmaz.Nitelik,
                    KoordinatBilgileri = tasinmaz.KoordinatBilgileri,
                    MahalleId = tasinmaz.MahalleId,
                    MahalleAdi = mahalle.MahalleAdi,
                    KullaniciId = tasinmaz.KullaniciId,
                    KullaniciAdi = kullanici.Ad + " " + kullanici.Soyad,
                    IlAdi = il?.IlAdi ?? string.Empty,
                    IlceAdi = ilce?.IlceAdi ?? string.Empty
                });
            }
            return tasinmazlarDto;
        }

        public async Task<string> GetKullaniciAdiById(int kullaniciId)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
            if (kullanici == null)
            {
                throw new KeyNotFoundException($"Kullanici with id {kullaniciId} not found.");
            }
            return kullanici.Ad + " " + kullanici.Soyad;
        }

        public async Task<string> GetMahalleAdiById(int mahalleId)
        {
            var mahalle = await _context.Mahalleler.FindAsync(mahalleId);
            if (mahalle == null)
            {
                throw new KeyNotFoundException($"Mahalle with id {mahalleId} not found.");
            }
            return mahalle.MahalleAdi;
        }

        public async Task<IEnumerable<TasinmazDto>> GetPaginated(int skip, int take)
        {
            var tasinmazlar = await _context.Tasinmazlar
                .OrderBy(t => t.KoordinatBilgileri)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var tasinmazlarDto = new List<TasinmazDto>();
            foreach (var tasinmaz in tasinmazlar)
            {
                var mahalle = await _context.Mahalleler.FindAsync(tasinmaz.MahalleId);
                if (mahalle == null)
                {
                    throw new KeyNotFoundException($"Mahalle with id {tasinmaz.MahalleId} not found.");
                }
                var kullanici = await _context.Kullanicilar.FindAsync(tasinmaz.KullaniciId);
                if (kullanici == null)
                {
                    throw new KeyNotFoundException($"Kullanici with id {tasinmaz.KullaniciId} not found.");
                }
                var ilce = await _context.Ilceler.FindAsync(mahalle.IlceId);
                var il = await _context.Iller.FindAsync(ilce?.IlId);
                tasinmazlarDto.Add(new TasinmazDto
                {
                    Id = tasinmaz.Id,
                    Ada = tasinmaz.Ada,
                    Parsel = tasinmaz.Parsel,
                    Nitelik = tasinmaz.Nitelik,
                    KoordinatBilgileri = tasinmaz.KoordinatBilgileri,
                    MahalleId = tasinmaz.MahalleId,
                    MahalleAdi = mahalle.MahalleAdi,
                    KullaniciId = tasinmaz.KullaniciId,
                    KullaniciAdi = kullanici.Ad + " " + kullanici.Soyad,
                    IlAdi = il?.IlAdi ?? string.Empty,
                    IlceAdi = ilce?.IlceAdi ?? string.Empty
                });
            }
            return tasinmazlarDto;
        }

        public async Task<TasinmazDto> Update(TasinmazDto entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var tasinmaz = await _context.Tasinmazlar.FindAsync(entity.Id)
                ?? throw new KeyNotFoundException($"Tasinmaz with id {entity.Id} not found.");
            tasinmaz.Nitelik = entity.Nitelik;
            tasinmaz.Ada = entity.Ada;
            tasinmaz.Parsel = entity.Parsel;
            tasinmaz.MahalleId = entity.MahalleId;
            await _context.SaveChangesAsync();

            var mahalle = await _context.Mahalleler.FindAsync(tasinmaz.MahalleId)
                ?? throw new KeyNotFoundException($"Mahalle with id {tasinmaz.MahalleId} not found.");
            var kullanici = await _context.Kullanicilar.FindAsync(tasinmaz.KullaniciId)
                ?? throw new KeyNotFoundException($"Kullanici with id {tasinmaz.KullaniciId} not found.");

            var ilce = await _context.Ilceler.FindAsync(mahalle.IlceId);
            var il = await _context.Iller.FindAsync(ilce?.IlId);
            return new TasinmazDto
            {
                Id = tasinmaz.Id,
                Ada = tasinmaz.Ada,
                Parsel = tasinmaz.Parsel,
                Nitelik = tasinmaz.Nitelik,
                KoordinatBilgileri = tasinmaz.KoordinatBilgileri,
                MahalleId = tasinmaz.MahalleId,
                MahalleAdi = mahalle.MahalleAdi,
                KullaniciId = tasinmaz.KullaniciId,
                KullaniciAdi = kullanici.Ad + " " + kullanici.Soyad,
                IlAdi = il?.IlAdi ?? string.Empty,
                IlceAdi = ilce?.IlceAdi ?? string.Empty
            };
        }
    }
}