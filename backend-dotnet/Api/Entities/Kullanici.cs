using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entities
{
    public class Kullanici
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "E-posta adresi boş bırakılamaz.")]
        [MaxLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public required string Email { get; set; }

        [MinLength(8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).+$", ErrorMessage = "Şifre en az bir harf, bir rakam ve bir özel karakter içermelidir.")]
        [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
        public required string Sifre { get; set; }

        [Required(ErrorMessage = "Rol seçilmelidir.")]
        public int RolId { get; set; }

        [MaxLength(30)]
        public string Ad { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Soyad { get; set; } = string.Empty;

        public bool Aktif { get; set; }
    }
}