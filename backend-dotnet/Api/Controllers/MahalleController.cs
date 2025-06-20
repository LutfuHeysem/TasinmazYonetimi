using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.DTOs;
using Api.Entities;
using Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MahalleController : ControllerBase
    {
        private readonly IMahalleService _mahalleService;
        private readonly ILogService _logService;
        private readonly ApplicationDbContext _context;
        public MahalleController(IMahalleService mahalleService, ILogService logService, ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mahalleService = mahalleService ?? throw new ArgumentNullException(nameof(mahalleService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddMahalleDto mahalleDto)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var user = _context.Kullanicilar.FirstOrDefault(u => u.Ad == User.Identity.Name) ?? null;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (user == null || user.RolId != 1)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 1,
                    Aciklama = "Yetkisiz mahalle ekleme denemesi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                });
                return Forbid();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var ilce = _context.Ilceler.FirstOrDefault(i => i.IlceAdi.ToUpper() == mahalleDto.IlceAdi.ToUpper());
                    if (ilce == null)
                    {
                        await _logService.Add(new Log()
                        {
                            KullaniciAdi = User.Identity?.Name ?? "Unknown",
                            KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                            DurumId = 2,
                            IslemTipId = 1,
                            Aciklama = $"Mahalle ekleme isteği geçersiz, ilçe bulunamadı: {mahalleDto.IlceAdi}.",
                            TarihSaat = DateTime.UtcNow,
                            KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                        });
                        return BadRequest("Ilçe bulunamadı.");
                    }
                    var mahalle = new Mahalle
                    {
                        MahalleAdi = mahalleDto.MahalleAdi,
                        IlceId = ilce.Id
                    };
                    var eklenenMahalle = await _mahalleService.Add(mahalle);
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 1,
                        Aciklama = $"{eklenenMahalle.MahalleAdi} mahallesi eklendi.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return Ok(eklenenMahalle);
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 1,
                        Aciklama = "Mahalle ekleme isteği geçersiz.",
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
                    Aciklama = $"Mahalle eklenirken hata oluştu: {ex.Message}",
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var user = _context.Kullanicilar.FirstOrDefault(u => u.Ad == User.Identity.Name) ?? null;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (user == null || user.RolId != 1)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 4,
                    Aciklama = "Yetkisiz mahalle silme denemesi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                });
                return Forbid();
            }
            try
            {
                var mahalle = await _mahalleService.GetById(id);
                if (mahalle == null)
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 4,
                        Aciklama = $"Mahalle silme isteği geçersiz, ID: {id} bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
                await _mahalleService.Delete(id);
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 4,
                    Aciklama = $"{mahalle.MahalleAdi} mahallesi silindi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok();
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 4,
                    Aciklama = $"Mahalle silinirken hata oluştu: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var mahalleler = await _mahalleService.GetAll();
                if (mahalleler == null || !mahalleler.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = "Mahalleler bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "Mahalleler başarıyla alındı.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(mahalleler);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Mahalleler alınırken hata oluştu: {ex.Message}",
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
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var mahalle = await _mahalleService.GetById(id);
                if (mahalle == null)
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = $"Mahalle bulunamadı, ID: {id}.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = $"{mahalle.MahalleAdi} mahallesi başarıyla alındı.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(mahalle);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Mahalle alınırken hata oluştu: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<int> GetCount()
        {
            try
            {
                var count = await _mahalleService.GetCount();
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = $"Toplam mahalle sayısı: {count}.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return count;
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Mahalle sayısı alınırken hata oluştu: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return -1;
            }
        }

        [HttpGet]
        [Route("{skip:int}/{take:int}")]
        [Authorize]
        public async Task<IActionResult> GetPaginated(int skip, int take)
        {
            try
            {
                var mahalleler = await _mahalleService.GetPaginated(skip, take);
                if (mahalleler == null || !mahalleler.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = "Sayfalı mahalleler bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "Sayfalı mahalleler başarıyla alındı.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(mahalleler);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Sayfalı mahalleler alınırken hata oluştu: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateMahalleDto updateMahalleDto)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var user = _context.Kullanicilar.FirstOrDefault(u => u.Ad == User.Identity.Name) ?? null;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (user == null || user.RolId != 1)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 3,
                    Aciklama = "Yetkisiz mahalle guncelleme denemesi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                });
                return Forbid();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var mahalle = await _context.Mahalleler.FirstOrDefaultAsync(m => m.MahalleAdi.ToUpper() == updateMahalleDto.MahalleAdi.ToUpper());
                    if (mahalle == null)
                    {
                        await _logService.Add(new Log()
                        {
                            KullaniciAdi = User.Identity?.Name ?? "Unknown",
                            KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                            DurumId = 2,
                            IslemTipId = 3,
                            Aciklama = $"Mahalle güncelleme isteği geçersiz, mahalle bulunamadı: {updateMahalleDto.MahalleAdi}.",
                            TarihSaat = DateTime.UtcNow,
                            KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                                            ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                                            ?? "Unknown"
                        });
                        return NotFound();
                    }
                    var ilce = await _context.Ilceler.FirstOrDefaultAsync(i => i.IlceAdi.ToUpper() == updateMahalleDto.IlceAdi.ToUpper());
                    if (ilce == null)
                    {
                        await _logService.Add(new Log()
                        {
                            KullaniciAdi = User.Identity?.Name ?? "Unknown",
                            KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                            DurumId = 2,
                            IslemTipId = 3,
                            Aciklama = $"Mahalle güncelleme isteği geçersiz, ilçe bulunamadı: {updateMahalleDto.IlceAdi}.",
                            TarihSaat = DateTime.UtcNow,
                            KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                                            ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                                            ?? "Unknown"
                        });
                        return BadRequest("Ilçe bulunamadı.");
                    }
                    var mahalleDto = new MahalleDto
                    {
                        Id = mahalle.Id,
                        MahalleAdi = updateMahalleDto.MahalleYeniAdi,
                        IlceId = ilce.Id,
                        IlceAdi = updateMahalleDto.IlceAdi
                    };
                    var updatedMahalle = await _mahalleService.Update(mahalleDto);
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 3,
                        Aciklama = $"{updatedMahalle.MahalleAdi} mahallesi güncellendi.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return Ok(updatedMahalle);
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 3,
                        Aciklama = "Mahalle güncelleme isteği geçersiz.",
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
                    IslemTipId = 3,
                    Aciklama = $"Mahalle güncellenirken hata oluştu: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }
    }
}