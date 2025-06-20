using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class UpdateIlceDto
    {
        public int Id { get; set; }
        public required string IlceAdi { get; set; } = string.Empty;
        public int IlPlaka { get; set; }
    }
}