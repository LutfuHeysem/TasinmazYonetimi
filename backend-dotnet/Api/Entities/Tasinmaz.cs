using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Entities
{
    public class Tasinmaz
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Mahalle")]
        public int MahalleId { get; set; }
        public Mahalle Mahalle { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Ada alanı zorunludur.")]
        public required string Ada { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = "Parsel alanı zorunludur.")]
        public required string Parsel { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Nitelik alanı zorunludur.")]
        public required string Nitelik { get; set; }
        public required string KoordinatBilgileri { get; set; }

        [ForeignKey("KullaniciId")]
        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }
    }
}