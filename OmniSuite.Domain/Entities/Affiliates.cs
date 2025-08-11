using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniSuite.Domain.Entities
{
    public class Affiliates
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }   
        public User User { get; set; }
        public bool? IsMarketUser { get; set; }
        public string AffiliateCode { get; set; }
    }
}
