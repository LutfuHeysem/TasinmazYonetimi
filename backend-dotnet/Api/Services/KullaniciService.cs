using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.DTOs;
using Api.Entities;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class KullaniciService : IKullaniciService
    {
        private readonly ApplicationDbContext _context;
        public KullaniciService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<KullaniciDto> LoginAsync(string email, string password)
        {
            password = PasswordChecker.Sifreleme(password);
            var kullanici = await _context.Kullanicilar
                .Where(k => k.Email == email && k.Sifre == password && k.Aktif == true)
                .FirstOrDefaultAsync();
            return kullanici != null ? new KullaniciDto
            {
                Id = kullanici.Id,
                Email = kullanici.Email,
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                RolId = kullanici.RolId,
                Aktif = kullanici.Aktif
            } : throw new UnauthorizedAccessException("Geçersiz email veya şifre.");
        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto, int id)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }
            kullanici.Sifre = PasswordChecker.Sifreleme(changePasswordDto.YeniSifre);
            _context.Kullanicilar.Update(kullanici);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CheckPasswordAsync(int id, string password)
        {
            password = PasswordChecker.Sifreleme(password);
            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }
            return kullanici.Sifre == password;
        }
        public async Task<Kullanici> GetById(int id)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id) ?? throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            return kullanici;
        }
        public async Task<IEnumerable<KullaniciDto>> GetAll()
        {
            var kullanicilar = await _context.Kullanicilar.ToListAsync();
            return kullanicilar.Select(k => new KullaniciDto
            {
                Id = k.Id,
                Email = k.Email,
                Ad = k.Ad,
                Soyad = k.Soyad,
                RolId = k.RolId,
                Aktif = k.Aktif
            });
        }
        public async Task<IEnumerable<KullaniciDto>> GetPaginated(int skip, int take)
        {
            var kullanicilar = await _context.Kullanicilar
                .Where(k => k.Aktif)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return kullanicilar.Select(k => new KullaniciDto
            {
                Id = k.Id,
                Email = k.Email,
                Ad = k.Ad,
                Soyad = k.Soyad,
                RolId = k.RolId,
                Aktif = k.Aktif
            });
        }

        public async Task<int> GetCount()
        {
            return await _context.Kullanicilar.Where(k => k.Aktif).CountAsync();
        }

        public Task<KullaniciDto> Add(Kullanici entity)
        {
            entity.Aktif = true;
            _context.Kullanicilar.Add(entity);
            return Task.FromResult(new KullaniciDto
            {
                Id = entity.Id,
                Email = entity.Email,
                Ad = entity.Ad,
                Soyad = entity.Soyad,
                RolId = entity.RolId,
                Aktif = entity.Aktif
            });
        }

        public async Task<KullaniciDto> AddDto(AddKullaniciDto entity)
        {
            var kullanici = new Kullanici()
            {
                Email = entity.Email,
                Ad = entity.Ad,
                Soyad = entity.Soyad,
                Sifre = PasswordChecker.Sifreleme(entity.Sifre),
                RolId = entity.RolId,
                Aktif = true
            };
            _context.Kullanicilar.Add(kullanici);
            await _context.SaveChangesAsync();
            return new KullaniciDto
            {
                Id = kullanici.Id,
                Email = kullanici.Email,
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                RolId = kullanici.RolId,
                Aktif = kullanici.Aktif
            };
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Kullanicilar.FindAsync(id);
            if (entity != null)
            {
                var tasinmazlar = _context.Tasinmazlar.Where(t => t.KullaniciId == entity.Id);
                foreach (var tasinmaz in tasinmazlar)
                    _context.Tasinmazlar.Remove(tasinmaz);
                entity.Aktif = false;
                _context.Kullanicilar.Update(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }
        }

        public Task<UpdateKullaniciDto> Update(UpdateKullaniciDto entityDto)
        {
            return Task.Run(async () =>
            {
                var kullanici = await _context.Kullanicilar.FindAsync(entityDto.Id);
                if (kullanici == null)
                {
                    throw new KeyNotFoundException("Kullanıcı bulunamadı.");
                }

                kullanici.Ad = entityDto.Ad;
                kullanici.Soyad = entityDto.Soyad;
                kullanici.Email = entityDto.Email;
                kullanici.RolId = entityDto.RolId;
                kullanici.Aktif = entityDto.Aktif;
                if (!string.IsNullOrEmpty(entityDto.Sifre))
                {
                    if (!PasswordChecker.IsPasswordCompliant(entityDto.Sifre))
                    {
                        throw new ArgumentException("Şifre kurallara uygun değil.");
                    }
                    kullanici.Sifre = PasswordChecker.Sifreleme(entityDto.Sifre);
                }

                _context.Kullanicilar.Update(kullanici);
                await _context.SaveChangesAsync();

                return entityDto;
            });
        }

        public Task<KullaniciDto> Update(KullaniciDto entityDto)
        {
            throw new NotImplementedException();
        }

        public async Task<KullaniciDto> GetByName(string kullaniciAdi)
        {
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Ad == kullaniciAdi);
            if (kullanici == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }
            return new KullaniciDto
            {
                Id = kullanici.Id,
                Email = kullanici.Email,
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                RolId = kullanici.RolId,
                Aktif = kullanici.Aktif
            };
        }
        
        public async Task<KullaniciDto?> GetByEmail(string email)
        {
            var kullanici = await _context.Kullanicilar.Where(k => k.Aktif == true).FirstOrDefaultAsync(k => k.Email == email);
            if (kullanici == null)
            {
                return null;
            }
            return new KullaniciDto
            {
                Id = kullanici.Id,
                Email = kullanici.Email,
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                RolId = kullanici.RolId,
                Aktif = kullanici.Aktif
            };
        }
    }
}