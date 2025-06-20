using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Entities
{
    public class Log()
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı Adı boş bırakılamaz.")]
        [MaxLength(50)]
        public required string KullaniciAdi { get; set; }

        [ForeignKey("Kullanici")]
        public int? KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }

        [ForeignKey("Durum")]
        public int DurumId { get; set; }
        public Durum Durum { get; set; }


        [ForeignKey("IslemTip")]
        public int IslemTipId { get; set; }
        public IslemTip IslemTip { get; set; }


        [MaxLength(3000)]
        [Required(ErrorMessage = "Açıklama boş bırakılamaz.")]
        public required string Aciklama { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Geçerli bir tarih ve saat giriniz.")]
        [Required(ErrorMessage = "Tarih ve saat boş bırakılamaz.")]
        public required DateTime TarihSaat { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Kullanıcı IP adresi boş bırakılamaz.")]
        public required string KullaniciIp { get; set; }
    }

}