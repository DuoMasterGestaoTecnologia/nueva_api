using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniSuite.Domain.Entities
{
    public class AffiliatesCommission
    {
        public Guid Id { get; set; }
        public Guid AffiliatesId { get; set; }
        public Affiliates Affiliates { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public long Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public TypeCommission Type { get; set; }
    }
}
