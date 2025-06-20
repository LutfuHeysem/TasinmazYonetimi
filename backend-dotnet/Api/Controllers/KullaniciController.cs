using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Interfaces;
using Api.Entities;
using Api.DTOs;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KullaniciController : ControllerBase
    {
        private readonly IKullaniciService _kullaniciService;
        private readonly ILogService _logService;
        public KullaniciController(IKullaniciService kullaniciService, ILogService logService)
        {
            _kullaniciService = kullaniciService;
            _logService = logService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddKullaniciDto kayit)
        {
            string kullaniciAdi = User.Identity?.Name ?? "Unknown";
            var kullanici = await _kullaniciService.GetByName(kullaniciAdi);
            if (kullanici.RolId != 1)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = kullaniciAdi,
                    DurumId = 2,
                    IslemTipId = 1,
                    Aciklama = $"{kullaniciAdi} kullanıcısı yetkisiz ekleme denemesi yaptı.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Unauthorized("Yetkiniz yok.");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    if (PasswordChecker.SifreKontrol(kayit.Sifre) == kayit.Sifre)
                    {
                        if(await _kullaniciService.GetByEmail(kayit.Email) != null)
                        {
                            await _logService.Add(new Log()
                            {
                                KullaniciAdi = User.Identity?.Name ?? "Unknown",
                                KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                                DurumId = 2,
                                IslemTipId = 1,
                                Aciklama = $"{kayit.Email} e-posta adresi zaten kayıtlı.",
                                TarihSaat = DateTime.UtcNow,
                                KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                                ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                                ?? "Unknown"
                            });
                            return Conflict("E-posta adresi zaten kayıtlı.");
                        }
                        var eklenenKullanici = await _kullaniciService.AddDto(kayit);
                        await _logService.Add(new Log()
                        {
                            KullaniciAdi = User.Identity?.Name ?? "Unknown",
                            KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                            DurumId = 1,
                            IslemTipId = 1,
                            Aciklama = $"{eklenenKullanici.Ad} kullanıcısı eklendi.",
                            TarihSaat = DateTime.UtcNow,
                            KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                        });
                        return CreatedAtAction(nameof(Add), new { id = eklenenKullanici.Id }, eklenenKullanici);
                    }
                    else
                    {
                        await _logService.Add(new Log()
                        {
                            KullaniciAdi = User.Identity?.Name ?? "Unknown",
                            KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                            DurumId = 2,
                            IslemTipId = 1,
                            Aciklama = $"{kayit.Ad} kullanıcısı eklenemedi. Şifre kriterlere uymuyor.",
                            TarihSaat = DateTime.UtcNow,
                            KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                        });
                        return BadRequest();
                    }
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 1,
                        Aciklama = $"{kayit.Ad} kullanıcısı eklenemedi. Bilgiler Yetersiz.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 1,
                    Aciklama = $"{kayit.Ad} kullanıcısı eklenemedi. Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            string kullaniciAdi = User.Identity?.Name ?? "Unknown";
            var kullanici = await _kullaniciService.GetByName(kullaniciAdi);
            if (kullanici.RolId != 1)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = kullaniciAdi,
                    DurumId = 2,
                    IslemTipId = 4,
                    Aciklama = $"{kullaniciAdi} kullanıcısı yetkisiz silme denemesi yaptı.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Unauthorized("Yetkiniz yok.");
            }
            try
            {
                if (await _kullaniciService.GetById(id) != null)
                {
                    await _kullaniciService.Delete(id);
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 4,
                        Aciklama = $"{id} ID'li kullanıcı silindi.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return Ok();
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 4,
                        Aciklama = $"{id} ID'li kullanıcı bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
            }
            catch (System.Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 4,
                    Aciklama = $"{id} ID'li kullanıcı silinemedi. Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value, out int kullaniciId);
                var kullanicilar = await _kullaniciService.GetAll();
                if (kullanicilar != null && kullanicilar.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = kullaniciId,
                        DurumId = 1,
                        IslemTipId = 2,
                        Aciklama = "Tüm kullanıcılar alındı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return Ok(kullanicilar);
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = "Kullanıcılar bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Kullanıcılar alınamadı. Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetPaginated")]
        public async Task<IActionResult> GetPaginated([FromQuery] int skip, [FromQuery] int take)
        {
            try
            {
                var kullanicilar = await _kullaniciService.GetPaginated(skip, take);
                if (kullanicilar != null && kullanicilar.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 2,
                        Aciklama = "Kullanıcılar alındı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    var count = await _kullaniciService.GetCount();
                    return Ok(new
                    {
                        total = count,
                        data = kullanicilar
                    });
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = "Kullanıcılar bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Kullanıcılar alınamadı. Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var kullanici = await _kullaniciService.GetById(id);
                if (kullanici != null)
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 2,
                        Aciklama = $"{id} ID'li kullanıcı alındı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return Ok(kullanici);
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = $"{id} ID'li kullanıcı bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"{id} ID'li kullanıcı alınamadı. Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("count")]
        public async Task<int> GetCount()
        {
            try
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "Kullanıcı sayısı alındı.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return await _kullaniciService.GetCount();
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Kullanıcı sayısı alınamadı. Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return -1;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _logService.Add(new Log()
            {
                KullaniciAdi = User.Identity?.Name ?? "Unknown",
                KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                DurumId = 1,
                IslemTipId = 6,
                Aciklama = "Kullanıcı çıkış yaptı.",
                TarihSaat = DateTime.UtcNow,
                KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
            });
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 5,
                    Aciklama = "Giriş yapılamadı. Bilgiler yetersiz.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }

            var kullanici = await _kullaniciService.LoginAsync(loginDto.Email, loginDto.Password);
            if (kullanici == null)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 5,
                    Aciklama = "Giriş yapılamadı. Kullanıcı adı veya şifre yanlış.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Unauthorized("Kullanıcı adı veya şifre yanlış.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, kullanici.Ad),
                new Claim(ClaimTypes.Role, kullanici.RolId.ToString()),
                new Claim("KullaniciId", kullanici.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            await HttpContext.SignInAsync(
                "Cookies",
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                }
            );

            await _logService.Add(new Log()
            {
                KullaniciAdi = kullanici.Ad,
                KullaniciId = kullanici.Id,
                DurumId = 1,
                IslemTipId = 5,
                Aciklama = $"{kullanici.Ad} kullanıcısı giriş yaptı.",
                TarihSaat = DateTime.UtcNow,
                KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
            });

            return Ok(kullanici);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            int id = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value);
            if (!ModelState.IsValid)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 3,
                    Aciklama = $"{id} ID'li kullanıcı şifresi güncellenemedi. Bilgiler Yetersiz.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }

            var kullanici = await _kullaniciService.GetById(id);
            if (kullanici == null)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 3,
                    Aciklama = $"{id} ID'li kullanıcı bulunamadı.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return NotFound();
            }

            if (kullanici.Sifre != PasswordChecker.Sifreleme(changePasswordDto.MevcutSifre))
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 3,
                    Aciklama = $"{id} ID'li kullanıcının mevcut şifresi yanlış.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Unauthorized("Mevcut şifre yanlış.");
            }

            if (changePasswordDto.YeniSifre != changePasswordDto.YeniSifreTekrar)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 3,
                    Aciklama = $"{id} ID'li kullanıcının yeni şifreleri eşleşmiyor.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest("Yeni şifreler eşleşmiyor.");
            }

            await _kullaniciService.ChangePasswordAsync(changePasswordDto, id);
            await _logService.Add(new Log()
            {
                KullaniciAdi = User.Identity?.Name ?? "Unknown",
                KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                DurumId = 1,
                IslemTipId = 3,
                Aciklama = $"{id} ID'li kullanıcının şifresi güncellendi.",
                TarihSaat = DateTime.UtcNow,
                KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
            });
            return NoContent();
        }

        [HttpPut]
        [Route("Update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateKullaniciDto kullaniciDto)
        {
            string kullaniciAdi = User.Identity?.Name ?? "Unknown";
            var kullanici = await _kullaniciService.GetByName(kullaniciAdi);
            if (kullanici.RolId != 1)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = kullaniciAdi,
                    DurumId = 2,
                    IslemTipId = 1,
                    Aciklama = $"{kullaniciAdi} kullanıcısı yetkisiz güncelleme denemesi yaptı.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Unauthorized("Yetkiniz yok.");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var guncellenenKullanici = await _kullaniciService.Update(kullaniciDto);
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 1,
                        Aciklama = $"{guncellenenKullanici.Ad} kullanıcısı güncellendi.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return Ok(guncellenenKullanici);
                }
                return BadRequest("Model geçersiz.");
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 1,
                    Aciklama = $"{kullaniciDto.Ad} kullanıcısı güncellenemedi. Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest("Kullanıcı güncellenemedi.");
            }
        }
    }
}