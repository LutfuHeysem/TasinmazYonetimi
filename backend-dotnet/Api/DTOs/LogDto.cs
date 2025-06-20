using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Entities;

namespace Api.DTOs
{
    public class LogDto
    {
        public int Id { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public KullaniciDto Kullanici { get; set; }
        public Durum Durum { get; set; }
        public IslemTip IslemTip { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public required string Aciklama { get; set; }

        public required DateTime TarihSaat { get; set; }

        public required string KullaniciIp { get; set; }
    }
}