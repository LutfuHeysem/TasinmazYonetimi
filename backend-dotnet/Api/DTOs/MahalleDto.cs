using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class MahalleDto
    {
        public int Id { get; set; }
        public string MahalleAdi { get; set; } = string.Empty;
        public int IlceId { get; set; }
        public string IlceAdi { get; set; } = string.Empty;
    }
}