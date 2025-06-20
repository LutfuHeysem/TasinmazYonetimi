using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs
{
    public class AddIlceDto
    {
        public required string IlceAdi { get; set; }
        public int IlPlaka { get; set; }
    }
}