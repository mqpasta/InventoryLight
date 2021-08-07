using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCore.Models
{
    public class HelperFunctions
    {
        public static List<Product> CreateTempProducts()
        {
            List<Product> products = new List<Product>();

            products.Add(new Product()
            {
                ProductId = 1,
                ProductCode = 101,
                ProductName = "Glass Cover"
            });
            products.Add(new Product()
            {
                ProductId = 2,
                ProductCode = 105,
                ProductName = "Fancy Packing"
            });

            return products;
        }

        public static List<Location> CreateTempLocations()
        {
            List<Location> locations = new List<Location>();

            locations.Add(new Location()
            {
                LocationId = 1,
                LocationCode = 301,
                LocationName = "Wearhouse 1"
            });

            locations.Add(new Location()
            {
                LocationId = 2,
                LocationCode = 302,
                LocationName = "Wearhouse 2"
            });

            return locations;
        }

        public static List<PurchaseMovement> CreateTempPurchaseMovements()
        {
            List<PurchaseMovement> purchaseMovements = new List<PurchaseMovement>();

            purchaseMovements.Add(new PurchaseMovement()
            {
                MovementType = StockMovementType.Purchase,
                ProductId = 1,
                ToLocationId = 1,
                Quantity = 10,
                StockMovementId = 1
            });

            purchaseMovements.Add(new PurchaseMovement()
            {
                MovementType = StockMovementType.Purchase,
                ProductId = 2,
                ToLocationId = 1,
                Quantity = 20,
                StockMovementId = 2
            });

            purchaseMovements.Add(new PurchaseMovement() {
                MovementType = StockMovementType.Purchase,
                ProductId = 2,
                ToLocationId = 2,
                Quantity = 15,
                StockMovementId = 3
            });

            return purchaseMovements;
        }
    }
}
