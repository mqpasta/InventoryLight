using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestCore.Models;

namespace TestCore.Models.IRepository
{
    interface IPurchaseOrderRepository
    {
        void Add(PurchaseOrder purchaseOrder);
        void Edit(PurchaseOrder purchaseOrder);
        void Remove(long id);
        PurchaseOrder Find(long id);
        List<PurchaseOrder> GetOrders();
        int TotalReceived(long id);
    }
}
