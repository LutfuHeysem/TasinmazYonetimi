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
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasinmazController : ControllerBase
    {
        private readonly ITasinmazService _tasinmazService;
        private readonly ILogService _logService;
        private readonly ApplicationDbContext _context;
        public TasinmazController(ITasinmazService tasinmazService, ILogService logService, ApplicationDbContext context)
        {
            _tasinmazService = tasinmazService ?? throw new ArgumentNullException(nameof(tasinmazService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]
        [Authorize]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] AddTasinmazDto addTasinmazDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mahalle = await _context.Mahalleler.FirstOrDefaultAsync(m => m.MahalleAdi.ToUpper() == addTasinmazDto.MahalleAdi.ToUpper());
                    if (mahalle == null)
                    {
                        await _logService.Add(new Log()
                        {
                            KullaniciAdi = User.Identity?.Name ?? "Unknown",
                            KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                            DurumId = 2,
                            IslemTipId = 1,
                            Aciklama = $"Mahalle '{addTasinmazDto.MahalleAdi}' bulunamadı.",
                            TarihSaat = DateTime.UtcNow,
                            KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                        });
                        return NotFound();
                    }
                    var tasinmaz = new Tasinmaz()
                    {
                        MahalleId = mahalle.Id,
                        Mahalle = mahalle,
                        Ada = addTasinmazDto.Ada,
                        Parsel = addTasinmazDto.Parsel,
                        Nitelik = addTasinmazDto.Nitelik,
                        KoordinatBilgileri = addTasinmazDto.KoordinatBilgileri,
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        Kullanici = await _context.Kullanicilar.FindAsync(Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value))
                    };
                    var eklenenTasinmaz = await _tasinmazService.Add(tasinmaz);
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 1,
                        Aciklama = $"{eklenenTasinmaz.Id} ID'li taşınmaz eklendi.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return Ok(eklenenTasinmaz);
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 1,
                        Aciklama = "Taşınmaz ekleme işlemi başarısız. Model geçersiz.",
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
                    Aciklama = $"Taşınmaz ekleme işlemi sırasında hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("Iller")]
        [Authorize]
        public async Task<IActionResult> GetIller()
        {
            try
            {
                var ilceler = await _context.Mahalleler
                    .Select(m => new { m.IlceId, m.Ilce.IlceAdi })
                    .Distinct()
                    .ToListAsync();
                if (ilceler == null || !ilceler.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = "İlçe listesi boş.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
                var iller = new List<string>();
                foreach (var ilce in ilceler)
                {
                    var il = await _context.Ilceler
                        .Where(i => i.Id == ilce.IlceId)
                        .Select(i => i.Il.IlAdi)
                        .FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(il) && !iller.Contains(il))
                    {
                        iller.Add(il);
                    }
                }
                return Ok(iller);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"İlçeleri getirme işlemi sırasında hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("Ilceler")]
        [Authorize]
        public async Task<IActionResult> GetIlceler([FromQuery] string ilAdi)
        {
            try
            {
                var ilceler = await _context.Ilceler.ToListAsync();
                var returnIlceler = new List<string>();
                foreach (var ilce in ilceler)
                {
                    var il = await _context.Iller.FindAsync(ilce.IlId);
                    if (il != null)
                    {
                        if (il.IlAdi.Equals(ilAdi, StringComparison.OrdinalIgnoreCase))
                        {
                            returnIlceler.Add(ilce.IlceAdi);
                        }
                    }
                }
                if (returnIlceler == null || !returnIlceler.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = $"'{ilAdi}' iline ait ilçeler bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                    });
                    return NotFound();
                }
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 2,
                        Aciklama = $"'{ilAdi}' iline ait ilçeler başarıyla getirildi.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
        ?? HttpContext.Connection.RemoteIpAddress?.ToString()
        ?? "Unknown"
                    });
                    return Ok(returnIlceler);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"İlçeleri getirme işlemi sırasında hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("Mahalleler")]
        [Authorize]
        public async Task<IActionResult> GetMahalleler([FromQuery] string ilceAdi)
        {
            try
            {
                var mahalleler = await _context.Mahalleler.ToListAsync();
                var returnMahalleler = new List<string>();
                foreach (var mahalle in mahalleler)
                {
                    var ilce = await _context.Ilceler.FindAsync(mahalle.IlceId);
                    if (ilce != null && ilce.IlceAdi.Equals(ilceAdi, StringComparison.OrdinalIgnoreCase))
                    {
                        returnMahalleler.Add(mahalle.MahalleAdi);
                    }
                }
                if(returnMahalleler == null || !returnMahalleler.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = $"'{ilceAdi}' ilçesine ait mahalleler bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                    });
                    return NotFound();
                }
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = $"'{ilceAdi}' ilçesine ait mahalleler başarıyla getirildi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                });
                return Ok(returnMahalleler);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Mahalleleri getirme işlemi sırasında hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("Delete")]
        [Authorize]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            try
            {
                var silinenTasinmaz = await _tasinmazService.GetById(id);
                if (silinenTasinmaz == null)
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 4,
                        Aciklama = $"{id} ID'li taşınmaz bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
                await _tasinmazService.Delete(id);
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 4,
                    Aciklama = $"{id} ID'li taşınmaz silindi.",
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
                    Aciklama = $"Taşınmaz silme işlemi sırasında hata: {ex.Message}",
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
                var tasinmazlar = await _tasinmazService.GetAll();
                if (tasinmazlar == null || !tasinmazlar.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = "Taşınmaz listesi boş.",
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
                    Aciklama = "Taşınmaz listesi başarıyla getirildi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(tasinmazlar);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Taşınmazları listeleme işlemi sırasında hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/filter")]
        [Authorize]
        public async Task<IActionResult> GetAllFiltered([FromQuery] string filter)
        {
            try
            {
                var tasinmazlar = await _tasinmazService.GetAllFiltered(filter);
                if (tasinmazlar == null || !tasinmazlar.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = "Filtrelenmiş taşınmaz listesi boş.",
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
                    Aciklama = "Filtrelenmiş taşınmaz listesi başarıyla getirildi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(tasinmazlar);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Filtrelenmiş taşınmazları listeleme işlemi sırasında hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("Paged")]
        [Authorize]
        public async Task<IActionResult> GetAllPaginatedByYetki([FromQuery] int skip, [FromQuery] int take)
        {
            int kullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value);
            var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
            int yetkiId = kullanici?.RolId ?? 0;
            try
            {
                var tasinmazlar = await _tasinmazService.GetAllPaginatedByYetki(skip, take, kullaniciId, yetkiId);
                if (tasinmazlar == null || !tasinmazlar.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = kullaniciId,
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = "Sayfalı taşınmaz listesi boş.",
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
                    KullaniciId = kullaniciId,
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "Sayfalı taşınmaz listesi başarıyla getirildi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                var count = await _tasinmazService.GetCount();
                return Ok(new
                {
                    total = count,
                    data = tasinmazlar
                });
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = kullaniciId,
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Sayfalı taşınmazları listeleme işlemi sırasında hata: {ex.Message}",
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
                var tasinmaz = await _tasinmazService.GetById(id);
                if (tasinmaz == null)
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = $"{id} ID'li taşınmaz bulunamadı.",
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
                    Aciklama = $"{id} ID'li taşınmaz başarıyla getirildi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(tasinmaz);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Taşınmaz getirme işlemi sırasında hata: {ex.Message}",
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
                var count = await _tasinmazService.GetCount();
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "Taşınmaz sayısı başarıyla alındı.",
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
                    Aciklama = $"Taşınmaz sayısı alma işlemi sırasında hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return 0;
            }
        }

        [HttpGet]
        [Route("/filterCount")]
        [Authorize]
        public async Task<int> GetCountFiltered([FromQuery] string filter)
        {
            try
            {
                var count = await _tasinmazService.GetCountFiltered(filter);
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "Filtrelenmiş taşınmaz sayısı başarıyla alındı.",
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
                    Aciklama = $"Filtrelenmiş taşınmaz sayısı alma işlemi sırasında hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return 0;
            }
        }

        [HttpGet]
        [Route("FilterPaged")]
        [Authorize]
        public async Task<IActionResult> GetFilteredWithPaginationByYetki([FromQuery] string filter,
                                                                          [FromQuery] int pageNumber,
                                                                          [FromQuery] int pageSize)
        {
            int kullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value);
            var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
            int yetkiId = kullanici?.RolId ?? 0;
            try
            {
                var tasinmazlar = await _tasinmazService.GetFilteredWithPaginationByYetki(filter, pageNumber, pageSize, kullaniciId, yetkiId);
                if (tasinmazlar == null || !tasinmazlar.Any())
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = kullaniciId,
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = "Filtrelenmiş ve sayfalı taşınmaz listesi boş.",
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
                    KullaniciId = kullaniciId,
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "Filtrelenmiş ve sayfalı taşınmaz listesi başarıyla getirildi.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                var count = await _tasinmazService.GetCountFiltered(filter);
                return Ok(new
                {
                    total = count,
                    data = tasinmazlar
                });
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = kullaniciId,
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Filtrelenmiş ve sayfalı taşınmazları listeleme işlemi sırasında hata: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize]
        [Route("Update")]
        public async Task<IActionResult> Update([FromQuery] int id, [FromBody] UpdateTasinmazDto tasinmaz)
        {
            try
            {
                var guncellenenTasinmaz = await _tasinmazService.GetById(id);
                if (guncellenenTasinmaz == null)
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 3,
                        Aciklama = $"{id} ID'li taşınmaz bulunamadı.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    var tasinmazDto = new TasinmazDto()
                    {
                        Id = id,
                        MahalleAdi = tasinmaz.Mahalle,
                        Ada = tasinmaz.Ada,
                        Parsel = tasinmaz.Parsel,
                        Nitelik = tasinmaz.Nitelik,
                        KoordinatBilgileri = guncellenenTasinmaz.KoordinatBilgileri,
                        KullaniciId = guncellenenTasinmaz.KullaniciId,
                        KullaniciAdi = guncellenenTasinmaz.Kullanici?.Ad ?? await _tasinmazService.GetKullaniciAdiById(guncellenenTasinmaz.KullaniciId),
                        MahalleId = _context.Mahalleler
                            .Where(m => m.MahalleAdi.ToLower() == tasinmaz.Mahalle.ToLower())
                            .Select(m => m.Id)
                            .FirstOrDefault(),
                    };
                    var updatedTasinmaz = await _tasinmazService.Update(tasinmazDto);
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 3,
                        Aciklama = $"{updatedTasinmaz.Id} ID'li taşınmaz güncellendi.",
                        TarihSaat = DateTime.UtcNow,
                        KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                    });
                    return Ok(updatedTasinmaz);
                }
                else
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 3,
                        Aciklama = "Taşınmaz güncelleme işlemi başarısız. Model geçersiz.",
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
                    Aciklama = $"Taşınmaz güncelleme işlemi sırasında hata: {ex.Message}",
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