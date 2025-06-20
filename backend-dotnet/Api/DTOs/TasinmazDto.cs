using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class TasinmazDto
    {
        public int Id { get; set; }
        public int MahalleId { get; set; }
        public string Ada { get; set; } = string.Empty;
        public string Parsel { get; set; } = string.Empty;
        public string Nitelik { get; set; } = string.Empty;
        public string KoordinatBilgileri { get; set; } = string.Empty;
        public int KullaniciId { get; set; }
        // Optional: For display purposes
        public string IlAdi { get; set; } = string.Empty;
        public string IlceAdi { get; set; } = string.Empty;
        public string MahalleAdi { get; set; } = string.Empty;
        public string KullaniciAdi { get; set; } = string.Empty;
    }
}