using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class UpdateMahalleDto
    {
        public string MahalleAdi { get; set; } = string.Empty;
        public string MahalleYeniAdi { get; set; } = string.Empty;
        public string IlceAdi { get; set; } = string.Empty;
    }
}