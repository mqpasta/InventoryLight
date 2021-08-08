using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestCore.Models;
using TestCore.Models.IRepository;

namespace TestCore.Models.FakeRepository
{
    public class FakePurchaseRepository : IPurchaseRepository
    {
        public static List<StockMovement> StockMovements = new List<StockMovement>();
        public static List<StockStatus> StockStatus = new List<StockStatus>();

        public void Add(PurchaseMovement purchase)
        {
            purchase.MovementType = StockMovementType.Purchase;
            purchase.StockMovementId = new Random().Next(100, 500);
            StockMovements.Add(purchase);
        }

        public void Edit(PurchaseMovement purchase)
        {
            PurchaseMovement found = (PurchaseMovement)StockMovements.FirstOrDefault(
                x => x.StockMovementId == purchase.StockMovementId &&
                x.MovementType == StockMovementType.Purchase);

            if (found != null)
            {
                found.ProductId = purchase.ProductId;
                found.Quantity = purchase.Quantity;
                found.ToLocationId = purchase.ToLocationId;
            }
        }

        public PurchaseMovement Find(long id)
        {
            PurchaseMovement found = (PurchaseMovement)StockMovements.FirstOrDefault(
                x => x.StockMovementId == id &&
                x.MovementType == StockMovementType.Purchase);

            return found;
        }

        public IEnumerable GetPurchases(long purchaseOrderId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable GetPurchases()
        {
            var founds = StockMovements.FindAll(x => x.MovementType == StockMovementType.Purchase);

            List<PurchaseMovement> allFound = new List<PurchaseMovement>();
            foreach (StockMovement s in founds)
            {
                PurchaseMovement pm = s as PurchaseMovement;
                allFound.Add(pm);
            }


            return allFound;
        }

        public void Remove(long id)
        {
            var found = Find(id);

            if (found != null)
                StockMovements.Remove(found);
        }
    }
}
