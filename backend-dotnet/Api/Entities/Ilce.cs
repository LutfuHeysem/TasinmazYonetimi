using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Entities
{
    public class Ilce
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "İlçe adı boş bırakılamaz.")]
        public required string IlceAdi { get; set; }
        [ForeignKey("Il")]
        public int IlId { get; set; }
        public Il Il { get; set; } 
    }
}