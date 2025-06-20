using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;
        public LogController(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        [HttpGet]
        [Authorize]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var logs = await _logService.GetAllDto();
                return Ok(logs);
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetPaginated")]
        // [Authorize]
        public async Task<IActionResult> GetPaginated([FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            try
            {
                var logs = await _logService.GetPaginated(skip, take);
                var count = await _logService.GetCount();
                return Ok(new
                {
                    total = count,
                    data = logs
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var log = await _logService.GetById(id);
                if (log == null)
                {
                    return NotFound();
                }
                return Ok(log);
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCount()
        {
            try
            {
                var count = await _logService.GetCount();
                return Ok(count);
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{filter}")]
        [Authorize]
        public async Task<IActionResult> GetFilter(string filter)
        {
            try
            {
                var log = await _logService.GetFilter(filter);
                return Ok(log);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (System.Exception)
            {
                return BadRequest("An error occurred while retrieving the log.");
            }
        }

        [HttpGet]
        [Route("GetPaginatedFiltered")]
        [Authorize]
        public async Task<IActionResult> GetPaginatedFiltered([FromQuery] int skip, [FromQuery] int take, [FromQuery] string filter)
        {
            try
            {
                var logs = await _logService.GetPaginatedFiltered(skip, take, filter);
                var count = await _logService.GetCountFiltered(filter);
                return Ok(new
                {
                    total = count,
                    data = logs
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (System.Exception)
            {
                return BadRequest("An error occurred while retrieving the filtered logs.");
            }
        }

        [HttpGet]
        [Route("{filter}/count")]
        [Authorize]
        public async Task<IActionResult> GetCountFiltered(string filter)
        {
            try
            {
                var count = await _logService.GetCountFiltered(filter);
                return Ok(count);
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }
    }
}