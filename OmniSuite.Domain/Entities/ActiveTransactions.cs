using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniSuite.Domain.Entities
{
    public class ActiveTransactions
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime EntranceTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public long CurrentChartValue { get; set; }
        public BuySellEnum BuySellEnum { get; set; }
        public decimal InputValue { get; set; }
        public decimal OutputValue { get; set; }
    }
}
