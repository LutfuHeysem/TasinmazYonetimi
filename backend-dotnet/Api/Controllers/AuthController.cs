using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userName = User.Identity?.Name;

            var user = await _context.Kullanicilar.FirstOrDefaultAsync(u => u.Ad == userName);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(new
            {
                user.Id,
                user.Ad,
                user.Email,
                user.RolId
            });
        }
    }
}