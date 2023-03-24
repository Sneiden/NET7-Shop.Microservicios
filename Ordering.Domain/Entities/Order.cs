using Ordering.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Entities
{
    internal class Order : EntityBase, IMultiTenant
    {
        public string UserName { get; set; } = null!;
        public decimal TotalPrice { get; set; }

        //
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int PaymentMethod { get; set; }
        public Guid TenantId { get; set; }
    }
}
