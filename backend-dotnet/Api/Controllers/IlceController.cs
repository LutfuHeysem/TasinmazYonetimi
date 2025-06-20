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
    public class IlceController : ControllerBase
    {
        private readonly IIlceService _ilceService;
        private readonly ILogService _logService;
        private readonly ApplicationDbContext _context;
        public IlceController(IIlceService ilceService, ILogService logService, ApplicationDbContext context)
        {
            _ilceService = ilceService ?? throw new ArgumentNullException(nameof(ilceService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddIlceDto ilceDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var il = _context.Iller.FirstOrDefault(i => i.Plaka == ilceDto.IlPlaka);
                    if (il != null)
                    {
                        var ilce = new Ilce()

                        {
                            IlceAdi = ilceDto.IlceAdi,
                            IlId = ilceDto.IlPlaka,
                            Il = il
                        };

                        var addedIlce = await _ilceService.Add(ilce);
                        await _logService.Add(new Log()
                        {
                            KullaniciAdi = User.Identity?.Name ?? "Unknown",
                            KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                            DurumId = 1,
                            IslemTipId = 1,
                            Aciklama = $"{addedIlce.IlceAdi} ilçe eklendi.",
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
                            IslemTipId = 1,
                            Aciklama = $"Il with plaka {ilceDto.IlPlaka} not found.",
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
                        IslemTipId = 1,
                        Aciklama = "Model state is invalid.",
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
                    Aciklama = $"Error adding ilce: {ex.Message}",
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
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value);
            var user = _context.Kullanicilar.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RolId == 1)
            {
                try
                {
                    var ilce = await _ilceService.GetById(id);
                    if (ilce == null)
                    {
                        await _logService.Add(new Log()
                        {
                            KullaniciAdi = User.Identity?.Name ?? "Unknown",
                            KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                            DurumId = 2,
                            IslemTipId = 4,
                            Aciklama = $"Ilce with id {id} not found.",
                            TarihSaat = DateTime.UtcNow,
                            KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
        ?? HttpContext.Connection.RemoteIpAddress?.ToString()
        ?? "Unknown"
                        });
                        return NotFound();
                    }
                    await _ilceService.Delete(id);
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 4,
                        Aciklama = $"{ilce.IlceAdi} ilçe silindi.",
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
                        Aciklama = $"Error deleting ilce: {ex.Message}",
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
                    KullaniciId = userId,
                    DurumId = 2,
                    IslemTipId = 4,
                    Aciklama = "Unauthorized access attempt to delete ilce.",
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
                var ilceler = await _ilceService.GetAll();
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "All ilceler retrieved successfully.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(ilceler);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Error retrieving ilceler: {ex.Message}",
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
                var ilce = await _ilceService.GetById(id);
                if (ilce == null)
                {
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 2,
                        IslemTipId = 2,
                        Aciklama = $"Ilce with id {id} not found.",
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
                    Aciklama = $"{ilce.IlceAdi} ilçe retrieved successfully.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(ilce);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Error retrieving ilce: {ex.Message}",
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
        public async Task<IActionResult> GetCount()
        {
            try
            {
                var count = await _ilceService.GetCount();
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "Ilce count retrieved successfully.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(count);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Error retrieving ilce count: {ex.Message}",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("{skip:int}/{take:int}")]
        [Authorize]
        public async Task<IActionResult> GetPaginated(int skip, int take)
        {
            try
            {
                var ilceler = await _ilceService.GetPaginated(skip, take);
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 1,
                    IslemTipId = 2,
                    Aciklama = "Paginated ilceler retrieved successfully.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                });
                return Ok(ilceler);
            }
            catch (Exception ex)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                    DurumId = 2,
                    IslemTipId = 2,
                    Aciklama = $"Error retrieving paginated ilceler: {ex.Message}",
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
        public async Task<IActionResult> Update([FromBody] UpdateIlceDto ilce)
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value);
            var user = _context.Kullanicilar.FirstOrDefault(u => u.Id == userId);
            if (user == null || user.RolId != 1)
            {
                await _logService.Add(new Log()
                {
                    KullaniciAdi = User.Identity?.Name ?? "Unknown",
                    KullaniciId = userId,
                    DurumId = 2,
                    IslemTipId = 3,
                    Aciklama = "Unauthorized access attempt to update ilce.",
                    TarihSaat = DateTime.UtcNow,
                    KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                                    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                                    ?? "Unknown"
                });
                return Unauthorized();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var il = _context.Iller.FirstOrDefault(i => i.Plaka == ilce.IlPlaka);
                    if (il == null)
                    {
                        await _logService.Add(new Log()
                        {
                            KullaniciAdi = User.Identity?.Name ?? "Unknown",
                            KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                            DurumId = 2,
                            IslemTipId = 3,
                            Aciklama = $"Il with plaka {ilce.IlPlaka} not found.",
                            TarihSaat = DateTime.UtcNow,
                            KullaniciIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
    ?? HttpContext.Connection.RemoteIpAddress?.ToString()
    ?? "Unknown"
                        });
                        return NotFound();
                    }
                    var ilceDto = new IlceDto()
                    {
                        Id = ilce.Id,
                        IlceAdi = ilce.IlceAdi,
                        IlId = il.Id,
                        IlAdi = il.IlAdi
                    };
                    var updatedIlce = await _ilceService.Update(ilceDto);
                    await _logService.Add(new Log()
                    {
                        KullaniciAdi = User.Identity?.Name ?? "Unknown",
                        KullaniciId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "KullaniciId")?.Value),
                        DurumId = 1,
                        IslemTipId = 3,
                        Aciklama = $"{updatedIlce.IlceAdi} ilçe güncellendi.",
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
                        IslemTipId = 3,
                        Aciklama = "Model state is invalid.",
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
                    Aciklama = $"Error updating ilce: {ex.Message}",
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