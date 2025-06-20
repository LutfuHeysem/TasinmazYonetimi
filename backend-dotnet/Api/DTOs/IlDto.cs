using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class IlDto
    {
        public int Id { get; set; }
        public string IlAdi { get; set; } = string.Empty;
        public int Plaka { get; set; }
    }
}