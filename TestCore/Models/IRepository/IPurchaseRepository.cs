using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

using TestCore.Models;

namespace TestCore.Models.IRepository
{
    public interface IPurchaseRepository
    {
        void Add(PurchaseMovement purchase);
        void Edit(PurchaseMovement purchase);
        void Remove(long id);
        PurchaseMovement Find(long id);
        IEnumerable GetPurchases();
        IEnumerable GetPurchases(long purchaseOrderId);
    }
}
