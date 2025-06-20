using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class AddTasinmazDto
    {
        public string MahalleAdi { get; set; } = string.Empty;
        public string Ada { get; set; } = string.Empty;
        public string Parsel { get; set; } = string.Empty;
        public string Nitelik { get; set; } = string.Empty;
        public string KoordinatBilgileri { get; set; } = string.Empty;
    }
}