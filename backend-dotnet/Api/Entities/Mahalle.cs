using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api.Entities
{
    public class Mahalle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Mahalle adı boş bırakılamaz.")]
        public required string MahalleAdi { get; set; }

        [ForeignKey("Ilce")]
        public int IlceId { get; set; }
        public Ilce Ilce { get; set; }
    }
}