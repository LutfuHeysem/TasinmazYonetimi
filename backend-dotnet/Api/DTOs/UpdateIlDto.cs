using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class UpdateIlDto
    {
        public required string IlAdi { get; set; }
        public int Plaka { get; set; }
    }
}