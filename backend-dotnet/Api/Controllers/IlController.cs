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

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IlController : ControllerBase
    {
        private readonly IIlService _ilService;
        private readonly ILogService _logService;
        private readonly ApplicationDbContext _context;
        public IlController(IIlService ilService, ILogService logService, ApplicationDbContext context)
        {
            _ilService = ilService ?? throw new ArgumentNullException(nameof(ilService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] Il entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var addedEntity = await _ilService.Add(entity);
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 1,
                        Aciklama = $"{entity.IlAdi} ili eklendi.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return CreatedAtAction(nameof(Add), new { id = addedEntity.Id }, addedEntity);
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 1,
                        Aciklama = "Geçersiz model durumu.",
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
                    Aciklama = $"Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{plaka:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int plaka)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var user = _context.Kullanicilar.FirstOrDefault(u => u.Ad == User.Identity.Name);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (user != null && user.RolId == 1)
            {
                try
                {
                    var entity = await _ilService.GetByPlaka(plaka);
                    if (entity != null)
                    {
                        await _ilService.Delete(entity.Id);
                        await _logService.Add(new Log()
                        {
                            KullaniciAdi = User.Identity?.Name ?? "Unknown",
                            KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                            DurumId = 1,
                            IslemTipId = 4,
                            Aciklama = $"{plaka} plaka'lı il silindi.",
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
                            Aciklama = $"Hata: Il with plaka {plaka} not found.",
                            TarihSaat = DateTime.UtcNow,
                            KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
        ?? HttpContext.Connection.RemoteIpAddress?.ToString()
        ?? "Unknown"
                        });
                        return NotFound();
                    }
                }
                catch (KeyNotFoundException knfEx)
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 4,
                        Aciklama = $"Hata: {knfEx.Message}",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
        ?? HttpContext.Connection.RemoteIpAddress?.ToString()
        ?? "Unknown"
                    });
                    return NotFound();
                }
            }
            else
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 4,
                    Aciklama = $"Hata: Yetkisiz erişim.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
        ?? HttpContext.Connection.RemoteIpAddress?.ToString()
        ?? "Unknown"
                });
                return Unauthorized();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var entities = await _ilService.GetAll();
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "Tüm iller listelendi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(entities);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{plaka:int}")]
        [Authorize]
        public async Task<IActionResult> GetByPlaka(int plaka)
        {
            try
            {
                var entity = await _ilService.GetByPlaka(plaka);
                if (entity != null)
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 2,
                        Aciklama = $"{plaka} plaka numaralı il detayları görüntülendi.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return Ok(entity);
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = $"Hata: Il with plaka {plaka} not found.",
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
                    Aciklama = $"Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
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
                    Aciklama = "İl sayısı alındı.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return await _ilService.GetCount();
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return 0;
            }
        }

        [HttpGet]
        [Route("{skip:int}/{take:int}")]
        [Authorize]
        public async Task<IActionResult> GetPaginated(int skip, int take)
        {
            try
            {
                var entities = await _ilService.GetPaginated(skip, take);
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 3,
                    Aciklama = $"{skip} atlanarak {take} il listelendi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(entities);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return NotFound();
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateIlDto entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entityDto = new IlDto
                    {
                        Id = id,
                        IlAdi = entity.IlAdi,
                        Plaka = entity.Plaka
                    };
                    var updatedEntity = await _ilService.Update(entityDto);
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 5,
                        Aciklama = $"{id} id'li il güncellendi.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return Ok(updatedEntity);
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 5,
                        Aciklama = "Geçersiz model durumu.",
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
                    IslemTipId = 5,
                    Aciklama = $"Hata: {ex.Message}",
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