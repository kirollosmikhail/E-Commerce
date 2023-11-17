using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.order_Aggregate;

namespace Talabat.Core.Specifications.Order_Spec
{
    public class orderWithPaymentIntentSpec : BaseSpecifications<Order>
    {
        public orderWithPaymentIntentSpec(string PaymentIntentId):base(O=>O.PaymentIntentId==PaymentIntentId)
        {


            
        }

    }
}
