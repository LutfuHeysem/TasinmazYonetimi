using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class LoginDto
    {
        public required string Email { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
    }
}