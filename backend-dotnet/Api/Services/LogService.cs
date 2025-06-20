using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Entities;
using Api.DTOs;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;
        public LogService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Log> Add(Log log)
        {
            _context.Loglar.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<IEnumerable<LogDto>> GetAllDto()
        {
            var loglar = await _context.Loglar.Include(l => l.Durum).Include(l => l.IslemTip).OrderByDescending(l => l.TarihSaat).ToListAsync();
            var logDtos = new List<LogDto>();
            foreach (var log in loglar)
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                logDtos.Add(new LogDto()
                {
                    Id = log.Id,
                    Kullanici = _context.Kullanicilar
                        .Where(k => k.Id == log.KullaniciId)
                        .Select(k => k == null ? null : new KullaniciDto
                        {
                            Id = k.Id,
                            Email = k.Email,
                            Ad = k.Ad,
                            Soyad = k.Soyad,
                            RolId = k.RolId,
                            Aktif = k.Aktif
                        })
                        .FirstOrDefault(),
                    Aciklama = log.Aciklama,
                    TarihSaat = log.TarihSaat,
                    Durum = new Durum
                    {
                        Id = log.Durum.Id,
                        DurumAdi = log.Durum.DurumAdi
                    },
                    IslemTip = new IslemTip
                    {
                        Id = log.IslemTip.Id,
                        IslemAdi = log.IslemTip.IslemAdi
                    },
                    KullaniciIp = log.KullaniciIp
                });
#pragma warning restore CS8601 // Possible null reference assignment.
            }
            return logDtos;
        }
            
        public async Task<IEnumerable<LogDto>> GetPaginated(int skip, int take)
        {
            var loglar = await _context.Loglar.Include(l => l.Durum).Include(l => l.IslemTip)
                .OrderByDescending(l => l.TarihSaat)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var logDtos = loglar.Select(log => new LogDto
            {
                Id = log.Id,
                Kullanici = _context.Kullanicilar
                    .Where(k => k.Id == log.KullaniciId)
                    .Select(k => k == null ? null : new KullaniciDto
                    {
                        Id = k.Id,
                        Email = k.Email,
                        Ad = k.Ad,
                        Soyad = k.Soyad,
                        RolId = k.RolId,
                        Aktif = k.Aktif
                    })
                    .FirstOrDefault(),
                Aciklama = log.Aciklama,
                TarihSaat = log.TarihSaat,
                Durum = new Durum
                {
                    Id = log.Durum.Id,
                    DurumAdi = log.Durum.DurumAdi
                },
                IslemTip = new IslemTip
                {
                    Id = log.IslemTip.Id,
                    IslemAdi = log.IslemTip.IslemAdi
                },
                KullaniciIp = log.KullaniciIp
            });

            return logDtos;
        }

        public async Task<Log?> GetById(int id)
        {
            return await _context.Loglar.Include(l => l.Durum).Include(l => l.IslemTip)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<int> GetCount()
        {
            return await _context.Loglar.CountAsync();
        }

        public async Task<IEnumerable<Log>> GetFilter(string filter)
        {
            return await _context.Loglar.Include(l => l.Durum).Include(l => l.IslemTip)
                .Where(l => l.Aciklama.Contains(filter) || l.KullaniciAdi.Contains(filter) || l.IslemTip.IslemAdi.Contains(filter) || l.Durum.DurumAdi.Contains(filter))
                .OrderByDescending(l => l.TarihSaat)
                .ToListAsync() ?? throw new KeyNotFoundException("Log not found with the specified filter.");
        }

        public async Task<IEnumerable<LogDto>> GetPaginatedFiltered(int skip, int take, string filter)
        {
            filter = filter.Trim().ToLower();
            var loglar = await _context.Loglar.Include(l => l.Durum).Include(l => l.IslemTip)
                    .Where(l => l.Aciklama.ToLower().Contains(filter) ||
                                l.KullaniciAdi.ToLower().Contains(filter) ||
                                l.IslemTip.IslemAdi.ToLower().Contains(filter) ||
                                l.Durum.DurumAdi.ToLower().Contains(filter))
                    .OrderByDescending(l => l.TarihSaat)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync() ?? throw new KeyNotFoundException("No logs found with the specified filter.");

            var logDtos = loglar.Select(log => new LogDto
            {
                Id = log.Id,
                Kullanici = _context.Kullanicilar
                    .Where(k => k.Id == log.KullaniciId)
                    .Select(k => k == null ? null : new KullaniciDto
                    {
                        Id = k.Id,
                        Email = k.Email,
                        Ad = k.Ad,
                        Soyad = k.Soyad,
                        RolId = k.RolId,
                        Aktif = k.Aktif
                    })
                    .FirstOrDefault(),
                Aciklama = log.Aciklama,
                TarihSaat = log.TarihSaat,
                Durum = new Durum
                {
                    Id = log.Durum.Id,
                    DurumAdi = log.Durum.DurumAdi
                },
                IslemTip = new IslemTip
                {
                    Id = log.IslemTip.Id,
                    IslemAdi = log.IslemTip.IslemAdi
                },
                KullaniciIp = log.KullaniciIp
            });

            return logDtos;
        }

        public async Task<int> GetCountFiltered(string filter)
        {
            try
            {
                return await _context.Loglar
                    .Where(l => l.Aciklama.ToLower().Contains(filter) || l.KullaniciAdi.ToLower().Contains(filter) || l.IslemTip.IslemAdi.ToLower().Contains(filter) || l.Durum.DurumAdi.ToLower().Contains(filter))
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error counting filtered logs", ex);
            }
        }

        public Task<IEnumerable<Log>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}