using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class UpdateKullaniciDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public int RolId { get; set; }
        public bool Aktif { get; set; }
        public string Sifre { get; set; } = string.Empty;
    }
}