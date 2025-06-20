using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class IlceDto
    {
        public int Id { get; set; }
        public string IlceAdi { get; set; } = string.Empty;
        public int IlId { get; set; }
        public string IlAdi { get; set; } = string.Empty;
    }
}