using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class UpdateTasinmazDto
    {
        public string Ada { get; set; } = string.Empty;
        public string Parsel { get; set; } = string.Empty;
        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string Mahalle { get; set; } = string.Empty;
        public string Nitelik { get; set; } = string.Empty;
    }
}