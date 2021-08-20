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
        List<PurchaseOrder> Search(DateTime? startDate, DateTime? endDate,
                                    long? locationId, long? productId,
                                    bool? isReceived, bool? isBalanceQuantity);
        int TotalReceived(long id);
    }
}
