using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class ChangePasswordDto
    {
        public required string MevcutSifre { get; set; }
        public required string YeniSifre { get; set; }
        public required string YeniSifreTekrar { get; set; }
    }
}