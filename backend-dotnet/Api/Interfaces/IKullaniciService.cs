using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Entities;
using Api.DTOs;

namespace Api.Interfaces
{
    public interface IKullaniciService : IService<Kullanici, KullaniciDto>
    {
        Task<KullaniciDto> LoginAsync(string email, string password);
        Task<bool> LogoutAsync();
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto, int id);
        Task<bool> CheckPasswordAsync(int id, string password);
        Task<KullaniciDto> AddDto(AddKullaniciDto entity);
        Task<KullaniciDto> GetByName(string kullaniciAdi);
        Task<UpdateKullaniciDto> Update(UpdateKullaniciDto entity);
        Task<KullaniciDto?> GetByEmail(string email);
    }
}